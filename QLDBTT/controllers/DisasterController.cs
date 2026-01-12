using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class DisasterController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
		//Phần 1 : Lẫy dữ liệu và tìm kiếm
		// Lấy danh sách theo loại
		public List<DiemThienTai> GetListByType(string typeName)
		{
			string query = "SELECT * FROM ViewDiemThienTai WHERE LoaiThienTai = @Loai";
			SqlParameter[] param = { new SqlParameter("@Loai", typeName) };
			return ParseList(query, param);
		}

		// Tìm kiếm
		public List<DiemThienTai> Search(string typeName, string keyword)
		{
			string query = @"SELECT * FROM ViewDiemThienTai 
                             WHERE LoaiThienTai = @Loai 
                             AND (DonVi LIKE @Key OR Cap LIKE @Key)";

			SqlParameter[] param = {
				new SqlParameter("@Loai", typeName),
				new SqlParameter("@Key", "%" + keyword + "%")
			};
			return ParseList(query, param);
		}

		//Phần 2 Quản lý thêm sửa xoá

		public int Add(int donViId, string tenLoaiThienTai, int mucDo)
		{
			int loaiId = GetLoaiId(tenLoaiThienTai);
			if (loaiId == -1) return -1;

			// Dùng SCOPE_IDENTITY() để trả về ID vừa tạo (phục vụ upload file)
			string query = @"INSERT INTO DiemThienTai (DonViId, LoaiThienTaiId, MucDo) 
                             VALUES (@Dv, @Loai, @Muc);
                             SELECT CAST(SCOPE_IDENTITY() as int);";

			SqlParameter[] param = {
				new SqlParameter("@Dv", donViId),
				new SqlParameter("@Loai", loaiId),
				new SqlParameter("@Muc", mucDo)
			};

			object result = _dbHelper.ExecuteScalar(query, param);
			return result != null ? Convert.ToInt32(result) : -1;
		}

		public bool Update(int id, int donViId, int mucDo)
		{
			string query = "UPDATE DiemThienTai SET DonViId = @Dv, MucDo = @Muc WHERE Id = @Id";
			SqlParameter[] param = {
				new SqlParameter("@Dv", donViId),
				new SqlParameter("@Muc", mucDo),
				new SqlParameter("@Id", id)
			};
			return _dbHelper.ExecuteNonQuery(query, param) > 0;
		}

		public bool Delete(int id)
		{
			// 1. Xóa file đính kèm trước
			FileAttachmentHelper.DeleteFile(id);

			// 2. Xóa dữ liệu trong DB
			string query = "DELETE FROM DiemThienTai WHERE Id = @Id";
			return _dbHelper.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@Id", id) }) > 0;
		}

		// Helper: Upload file báo cáo
		public void UploadReportFile(int disasterId, string sourcePath)
		{
			if (disasterId > 0 && !string.IsNullOrEmpty(sourcePath))
			{
				FileAttachmentHelper.SaveFile(disasterId, sourcePath);
			}
		}

		// ---------------------------------------------------------
		// CÁC HÀM HỖ TRỢ (PRIVATE / HELPER)
		// ---------------------------------------------------------

		private List<DiemThienTai> ParseList(string query, SqlParameter[] p)
		{
			List<DiemThienTai> list = new List<DiemThienTai>();
			DataTable dt = _dbHelper.ExecuteQuery(query, p);
			foreach (DataRow row in dt.Rows)
			{
				int id = Convert.ToInt32(row["Id"]);
				bool hasFile = FileAttachmentHelper.HasAttachment(id);

				list.Add(new DiemThienTai
				{
					Id = id,
					TenDonVi = row["DonVi"].ToString(),
					CapHanhChinh = row["Cap"].ToString(),

					TenLoaiThienTai = row["LoaiThienTai"].ToString(),
					MucDo = Convert.ToInt32(row["MucDo"]),
					TrangThaiFile = hasFile ? "Đã có file" : "Chưa có"
				});
			}
			return list;
		}

		private int GetLoaiId(string tenLoai)
		{
			string query = "SELECT Id FROM LoaiThienTai WHERE Ten = @Ten";
			object result = _dbHelper.ExecuteScalar(query, new[] { new SqlParameter("@Ten", tenLoai) });
			return result != null ? Convert.ToInt32(result) : -1;
		}
	}
}