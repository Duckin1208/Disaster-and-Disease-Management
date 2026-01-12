using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class DiseaseController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		//Phần 1 Quản lý ổ dịch
		// Lấy danh sách ổ dịch (Hỗ trợ tìm kiếm theo từ khóa)
		public List<ODichHienThi> GetOutbreaks(string keyword = "")
		{
			// Sử dụng ViewODich mới đã cập nhật trong SQL
			string query = @"SELECT * FROM ViewODich 
                             WHERE TenDonVi LIKE @Key OR TenBenh LIKE @Key OR TrangThai LIKE @Key 
                             ORDER BY NgayPhatHien DESC";

			var param = new SqlParameter[] { new SqlParameter("@Key", "%" + keyword + "%") };

			var list = new List<ODichHienThi>();
			DataTable dt = _dbHelper.ExecuteQuery(query, param);

			foreach (DataRow row in dt.Rows)
			{
				list.Add(new ODichHienThi
				{
					Id = Convert.ToInt32(row["Id"]),
					DonViId = Convert.ToInt32(row["DonViId"]),
					TenDonVi = row["TenDonVi"].ToString(),
					CapHanhChinh = row["CapHanhChinh"].ToString(),

					LoaiDichBenhId = Convert.ToInt32(row["LoaiDichBenhId"]),
					TenBenh = row["TenBenh"].ToString(),

					// Xử lý các trường mới cập nhật
					NgayPhatHien = row["NgayPhatHien"] != DBNull.Value ? Convert.ToDateTime(row["NgayPhatHien"]) : DateTime.Now,
					SoLuongMacBenh = row["SoLuongMacBenh"] != DBNull.Value ? Convert.ToInt32(row["SoLuongMacBenh"]) : 0,
					SoLuongTieuHuy = row["SoLuongTieuHuy"] != DBNull.Value ? Convert.ToInt32(row["SoLuongTieuHuy"]) : 0,
					TrangThai = row["TrangThai"].ToString(),
					NguyenNhan = row["NguyenNhan"].ToString(), // (Req 3.4) Chẩn đoán
					DaTiemPhong = row["DaTiemPhong"] != DBNull.Value && Convert.ToBoolean(row["DaTiemPhong"]), // (Req 3.6) Tiêm phòng
					GhiChu = row["GhiChu"].ToString()
				});
			}
			return list;
		}

		// Thêm ổ dịch mới
		public bool AddOutbreak(int donViId, int loaiBenhId, DateTime ngayPhatHien, int soLuong, string nguyenNhan, bool daTiem)
		{
			string query = @"INSERT INTO ODich (DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, TrangThai, NguyenNhan, DaTiemPhong)
                             VALUES (@Dv, @Lb, @Ngay, @Sl, N'Đang lây lan', @NguyenNhan, @DaTiem)";

			SqlParameter[] p = {
				new SqlParameter("@Dv", donViId),
				new SqlParameter("@Lb", loaiBenhId),
				new SqlParameter("@Ngay", ngayPhatHien),
				new SqlParameter("@Sl", soLuong),
				new SqlParameter("@NguyenNhan", nguyenNhan),
				new SqlParameter("@DaTiem", daTiem)
			};
			return _dbHelper.ExecuteNonQuery(query, p) > 0;
		}

		// Xóa ổ dịch
		public bool DeleteOutbreak(int id)
		{
			return _dbHelper.ExecuteNonQuery("DELETE FROM ODich WHERE Id = " + id) > 0;
		}

		//Phần 2 Danh mục dịch bệnh và Triệu chứng
		// Lấy danh sách loại bệnh (Cho ComboBox)
		public List<LoaiDichBenh> GetLoaiBenh()
		{
			var list = new List<LoaiDichBenh>();
			DataTable dt = _dbHelper.ExecuteQuery("SELECT * FROM LoaiDichBenh");
			foreach (DataRow row in dt.Rows)
			{
				list.Add(new LoaiDichBenh
				{
					Id = Convert.ToInt32(row["Id"]),
					Ten = row["Ten"].ToString(),
					MoTa = row["MoTa"].ToString()
					// VatNuoi = row["VatNuoi"].ToString() (Bỏ comment nếu cần)
				});
			}
			return list;
		}
		//Thêm loại dịch bệnh
		public bool AddLoaiBenh(string ten, string moTa)
		{
			try
			{
				string query = "INSERT INTO LoaiDichBenh (Ten, MoTa) VALUES (@Ten, @MoTa)";
				return _dbHelper.ExecuteNonQuery(query, new[] {
			new SqlParameter("@Ten", ten),
			new SqlParameter("@MoTa", moTa ?? "") // Nếu mô tả null thì để trống
        }) > 0;
			}
			catch
			{
				return false;
			}
		}
		// =============================================================
		// PHẦN 3: BẢN ĐỒ (3.7)
		// =============================================================

		public List<MapDataPoint> GetMapData()
		{
			// Lấy dữ liệu tổng hợp từ ViewODich để vẽ bản đồ
			// Chỉ lấy những ổ dịch đang lây lan để cảnh báo
			string query = "SELECT TenDonVi, TenBenh, SoLuongMacBenh, TrangThai FROM ViewODich WHERE TrangThai = N'Đang lây lan'";

			var list = new List<MapDataPoint>();
			DataTable dt = _dbHelper.ExecuteQuery(query);

			foreach (DataRow row in dt.Rows)
			{
				list.Add(new MapDataPoint
				{
					TenDonVi = row["TenDonVi"].ToString(),
					TenLoaiDichBenh = row["TenBenh"].ToString(),
					// Logic màu: Nếu > 100 con thì báo Đỏ, ngược lại báo Vàng
					LevelColor = Convert.ToInt32(row["SoLuongMacBenh"]) > 100 ? "Red" : "Yellow",
					Info = $"{row["TenBenh"]} - {row["SoLuongMacBenh"]} con mắc bệnh"
				});
			}
			return list;
		}
	}
}