using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Controllers
{
	public class MapDataPoint
	{
		public string TenDonVi { get; set; }        // Tên địa điểm (Hà Nội, Ba Vì...)
		public string TenLoaiDichBenh { get; set; } // Tên bệnh
		public int SoLuong { get; set; }            // Số ca mắc
		public string LevelColor { get; set; }      // Mã màu (Red, Orange, Yellow, Green)
		public string Info { get; set; }            // Thông tin hiển thị (Tooltip)
	}

	public class DiseaseController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		// Lấy danh sách Loại bệnh (Cho ComboBox)
		public DataTable GetDiseaseTypes()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM LoaiDichBenh ORDER BY Ten");
		}

		// Thêm loại bệnh mới (Có kiểm tra trùng lặp)
		public bool AddDiseaseType(string ten, string moTa)
		{
			try
			{
				// Kiểm tra trùng tên
				string checkQuery = "SELECT COUNT(*) FROM LoaiDichBenh WHERE Ten = @Ten";
				int count = 0;
				object result = _dbHelper.ExecuteScalar(checkQuery, new[] { new SqlParameter("@Ten", ten) });

				if (result != null) int.TryParse(result.ToString(), out count);

				if (count > 0) return false; // Đã tồn tại

				// Thêm mới
				string query = "INSERT INTO LoaiDichBenh (Ten, MoTa) VALUES (@Ten, @MoTa)";
				SqlParameter[] param = {
					new SqlParameter("@Ten", ten),
					new SqlParameter("@MoTa", moTa ?? (object)DBNull.Value)
				};
				return _dbHelper.ExecuteNonQuery(query, param) > 0;
			}
			catch { return false; }
		}


		// Lấy danh sách ổ dịch (Hỗ trợ tìm kiếm)
		public DataTable GetAllOutbreaks(string keyword = "")
		{
			// Lấy dữ liệu từ ViewODich (Đã Join sẵn các bảng)
			string query = @"SELECT Id, TenODich, TenBenh, TenDonVi, NgayPhatHien, SoLuongMacBenh, 
                                    TrangThai, DaTiemPhong, ChanDoan 
                             FROM ViewODich WHERE 1=1";

			if (!string.IsNullOrEmpty(keyword))
			{
				query += " AND (TenBenh LIKE @Key OR TenDonVi LIKE @Key OR TenODich LIKE @Key)";
			}

			query += " ORDER BY NgayPhatHien DESC";

			return _dbHelper.ExecuteQuery(query, new[] {
				new SqlParameter("@Key", "%" + keyword + "%")
			});
		}

		// Lấy danh sách ổ dịch đơn giản (Chỉ ID và Tên) để nạp ComboBox bên Tiêm phòng
		public DataTable GetOutbreakList()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, TenODich FROM ODich ORDER BY Id DESC");
		}

		// Thêm ổ dịch mới
		public bool AddOutbreak(string tenODich, int loaiBenhId, int donViId, int soLuong, string nguyenNhan, string chanDoan, bool daTiem)
		{
			try
			{
				string query = @"INSERT INTO ODich (TenODich, LoaiDichBenhId, DonViId, SoLuongMacBenh, NguyenNhan, ChanDoan, DaTiemPhong, NgayPhatHien, TrangThai) 
                                 VALUES (@Ten, @Loai, @DonVi, @SL, @NN, @CD, @TP, GETDATE(), N'Đang xử lý')";

				SqlParameter[] param = {
					new SqlParameter("@Ten", tenODich),
					new SqlParameter("@Loai", loaiBenhId),
					new SqlParameter("@DonVi", donViId),
					new SqlParameter("@SL", soLuong),
					new SqlParameter("@NN", nguyenNhan ?? (object)DBNull.Value),
					new SqlParameter("@CD", chanDoan ?? (object)DBNull.Value),
					new SqlParameter("@TP", daTiem)
				};
				return _dbHelper.ExecuteNonQuery(query, param) > 0;
			}
			catch { return false; }
		}

		// Xóa ổ dịch
		public bool DeleteOutbreak(int id)
		{
			return _dbHelper.ExecuteNonQuery("DELETE FROM ODich WHERE Id = @Id", new[] {
				new SqlParameter("@Id", id)
			}) > 0;
		}

		// Lấy danh sách đợt tiêm phòng
		public DataTable GetVaccinations(string keyword = "")
		{
			string query = @"SELECT Id, TenDotTiem, TenBenh, TenODich, NgayTiem, LoaiVaccine, SoLuong, NguoiThucHien 
                             FROM ViewTiemPhong WHERE 1=1";

			if (!string.IsNullOrEmpty(keyword))
			{
				query += " AND (TenDotTiem LIKE @Key OR TenODich LIKE @Key)";
			}

			query += " ORDER BY NgayTiem DESC";

			return _dbHelper.ExecuteQuery(query, new[] {
				new SqlParameter("@Key", "%" + keyword + "%")
			});
		}

		// Thêm đợt tiêm phòng mới
		public bool AddVaccination(string tenDot, int loaiBenhId, int oDichId, DateTime ngayTiem, string vaccine, int soLuong, string nguoiTH)
		{
			try
			{
				string query = @"INSERT INTO TiemPhong (TenDotTiem, LoaiDichBenhId, ODichId, NgayTiem, LoaiVaccine, SoLuong, NguoiThucHien) 
                                 VALUES (@Ten, @Loai, @ODich, @Ngay, @Vac, @SL, @Nguoi)";

				SqlParameter[] param = {
					new SqlParameter("@Ten", tenDot),
					new SqlParameter("@Loai", loaiBenhId),
					new SqlParameter("@ODich", oDichId),
					new SqlParameter("@Ngay", ngayTiem),
					new SqlParameter("@Vac", vaccine),
					new SqlParameter("@SL", soLuong),
					new SqlParameter("@Nguoi", nguoiTH ?? (object)DBNull.Value)
				};
				return _dbHelper.ExecuteNonQuery(query, param) > 0;
			}
			catch { return false; }
		}

		// Xóa đợt tiêm phòng
		public bool DeleteVaccination(int id)
		{
			return _dbHelper.ExecuteNonQuery("DELETE FROM TiemPhong WHERE Id = @Id", new[] {
				new SqlParameter("@Id", id)
			}) > 0;
		}
		public List<MapDataPoint> GetMapData()
		{
			List<MapDataPoint> mapPoints = new List<MapDataPoint>();

			// Lấy toàn bộ dữ liệu ổ dịch
			DataTable dt = GetAllOutbreaks("");

			foreach (DataRow row in dt.Rows)
			{
				MapDataPoint point = new MapDataPoint();

				// 1. Lấy thông tin cơ bản từ View
				// Lưu ý: Cần chắc chắn ViewODich có cột TenDonVi, TenBenh, SoLuongMacBenh
				point.TenDonVi = row["TenDonVi"].ToString();
				point.TenLoaiDichBenh = row["TenBenh"].ToString();

				int sl = 0;
				if (row["SoLuongMacBenh"] != DBNull.Value)
					int.TryParse(row["SoLuongMacBenh"].ToString(), out sl);
				point.SoLuong = sl;

				// 2. Logic Phân Màu Cảnh Báo
				if (sl >= 100)
				{
					point.LevelColor = "#E74C3C"; // Đỏ (Rất nguy hiểm)
				}
				else if (sl >= 50)
				{
					point.LevelColor = "#E67E22"; // Cam (Nguy hiểm)
				}
				else if (sl >= 10)
				{
					point.LevelColor = "#F1C40F"; // Vàng (Cảnh báo)
				}
				else
				{
					point.LevelColor = "#2ECC71"; // Xanh lá (An toàn/Nhẹ)
				}

				// 3. Tạo thông tin Tooltip
				string ngayPH = row["NgayPhatHien"] != DBNull.Value
								? Convert.ToDateTime(row["NgayPhatHien"]).ToString("dd/MM/yyyy")
								: "N/A";

				point.Info = $"📍 Địa điểm: {point.TenDonVi}\n" +
							 $"🦠 Bệnh: {point.TenLoaiDichBenh}\n" +
							 $"⚠️ Số ca: {sl:N0}\n" +
							 $"📅 Ngày phát hiện: {ngayPH}";

				mapPoints.Add(point);
			}

			return mapPoints;
		}
	}
}