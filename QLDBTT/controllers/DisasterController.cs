using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class DisasterController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		// 1. Lấy danh sách Loại thiên tai
		public DataTable GetDisasterTypes()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM LoaiThienTai ORDER BY Ten");
		}

		// 2. Lấy danh sách Đơn vị hành chính
		public DataTable GetAdministrativeUnits()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM DonVi ORDER BY Ten");
		}

		// 3. Lấy danh sách Điểm thiên tai
		public List<DiemThienTai> GetAllDisasters(string keyword = "")
		{
			List<DiemThienTai> listResult = new List<DiemThienTai>();

			string query = @"SELECT * FROM ViewDiemThienTai WHERE 1=1";
			keyword = keyword?.Trim();

			if (!string.IsNullOrEmpty(keyword))
			{
				query += " AND (DonVi LIKE @Key OR LoaiThienTai LIKE @Key OR GhiChu LIKE @Key)";
			}

			query += " ORDER BY NgayGhiNhan DESC, Id DESC";

			DataTable dt = _dbHelper.ExecuteQuery(query, new[] {
				new SqlParameter("@Key", "%" + keyword + "%")
			});

			foreach (DataRow row in dt.Rows)
			{
				DiemThienTai item = new DiemThienTai();
				item.Id = Convert.ToInt32(row["Id"]);
				item.MucDo = row["MucDo"] != DBNull.Value ? Convert.ToInt32(row["MucDo"]) : 1;
				if (dt.Columns.Contains("DonVi"))
					item.TenDonVi = row["DonVi"].ToString();
				else if (dt.Columns.Contains("TenDonVi"))
					item.TenDonVi = row["TenDonVi"].ToString();

				if (dt.Columns.Contains("LoaiThienTai"))
					item.TenLoaiThienTai = row["LoaiThienTai"].ToString();
				else if (dt.Columns.Contains("TenLoaiThienTai"))
					item.TenLoaiThienTai = row["TenLoaiThienTai"].ToString();

				item.CapHanhChinh = dt.Columns.Contains("Cap") ? row["Cap"].ToString() : "";

				item.GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : "";

				if (row["NgayGhiNhan"] != DBNull.Value)
				{
					item.NgayGhiNhan = Convert.ToDateTime(row["NgayGhiNhan"]);
				}

				// Xử lý trạng thái File dựa trên Helper
				if (FileAttachmentHelper.HasAttachment(item.Id))
				{
					item.FileDinhKem = "Có"; // Gán giá trị giả để kích hoạt Property TrangThaiFile
				}
				else
				{
					item.FileDinhKem = "";
				}

				listResult.Add(item);
			}

			return listResult;
		}

		// 4. Thêm mới
		public bool AddDisaster(int donViId, int loaiThienTaiId, int mucDo, string ghiChu, string sourceFilePath)
		{
			try
			{
				// Insert vào DB
				string insertQuery = @"INSERT INTO DiemThienTai (DonViId, LoaiThienTaiId, MucDo, GhiChu, NgayGhiNhan) 
                                       VALUES (@Dv, @Loai, @Muc, @Note, GETDATE());
                                       SELECT CAST(SCOPE_IDENTITY() as int);";

				SqlParameter[] param = {
					new SqlParameter("@Dv", donViId),
					new SqlParameter("@Loai", loaiThienTaiId),
					new SqlParameter("@Muc", mucDo),
					new SqlParameter("@Note", ghiChu ?? (object)DBNull.Value)
				};

				object result = _dbHelper.ExecuteScalar(insertQuery, param);

				// Lưu file nếu có
				if (result != null && int.TryParse(result.ToString(), out int newId))
				{
					if (!string.IsNullOrEmpty(sourceFilePath) && File.Exists(sourceFilePath))
					{
						FileAttachmentHelper.SaveFile(newId, sourceFilePath);
						_dbHelper.ExecuteNonQuery("UPDATE DiemThienTai SET FileDinhKem = N'Có' WHERE Id = " + newId);
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Lỗi AddDisaster: " + ex.Message);
				return false;
			}
		}

		// 5. Xóa
		public bool DeleteDisaster(int id)
		{
			try
			{
				// Xóa file trước
				FileAttachmentHelper.DeleteFile(id);

				// Xóa DB sau
				string deleteQuery = "DELETE FROM DiemThienTai WHERE Id = @Id";
				return _dbHelper.ExecuteNonQuery(deleteQuery, new[] { new SqlParameter("@Id", id) }) > 0;
			}
			catch { return false; }
		}

		// 6. Mở file
		public void OpenAttachment(int id)
		{
			FileAttachmentHelper.OpenFile(id);
		}
	}
}