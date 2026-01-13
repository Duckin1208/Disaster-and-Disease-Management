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

		// 1. Lấy danh sách Loại thiên tai (Giữ DataTable cho ComboBox)
		public DataTable GetDisasterTypes()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM LoaiThienTai ORDER BY Ten");
		}

		// 2. Lấy danh sách Điểm thiên tai (QUAN TRỌNG: Trả về List<DiemThienTai>)
		// Trong file DisasterController.cs

		public List<DiemThienTai> GetAllDisasters(string keyword = "")
		{
			List<DiemThienTai> listResult = new List<DiemThienTai>();

			// Câu truy vấn
			string query = @"SELECT * FROM ViewDiemThienTai WHERE 1=1";
			keyword = keyword?.Trim();

			if (!string.IsNullOrEmpty(keyword))
			{
				query += " AND (DonVi LIKE @Key OR Cap LIKE @Key OR LoaiThienTai LIKE @Key)";
			}

			query += " ORDER BY Id DESC";

			DataTable dt = _dbHelper.ExecuteQuery(query, new[] {
		new SqlParameter("@Key", "%" + keyword + "%")
	});

			// Chuyển đổi DataTable sang List Object
			foreach (DataRow row in dt.Rows)
			{
				DiemThienTai item = new DiemThienTai();

				// --- PHẦN SỬA QUAN TRỌNG: KIỂM TRA CỘT TRƯỚC KHI ĐỌC ---

				// 1. Lấy ID (Bắt buộc phải có)
				item.Id = Convert.ToInt32(row["Id"]);

				// 2. Lấy DonViId (Nếu View thiếu cột này thì gán mặc định là 0 để không bị lỗi)
				if (dt.Columns.Contains("DonViId") && row["DonViId"] != DBNull.Value)
					item.DonViId = Convert.ToInt32(row["DonViId"]);
				else
					item.DonViId = 0; // Tránh crash

				// 3. Lấy LoaiThienTaiId
				if (dt.Columns.Contains("LoaiThienTaiId") && row["LoaiThienTaiId"] != DBNull.Value)
					item.LoaiThienTaiId = Convert.ToInt32(row["LoaiThienTaiId"]);
				else
					item.LoaiThienTaiId = 0;

				// 4. Lấy Mức Độ
				if (dt.Columns.Contains("MucDo") && row["MucDo"] != DBNull.Value)
					item.MucDo = Convert.ToInt32(row["MucDo"]);
				else
					item.MucDo = 1;

				// --- CÁC CỘT HIỂN THỊ (STRING) ---
				// Kiểm tra tên cột trong View của bạn là 'DonVi' hay 'TenDonVi'
				if (dt.Columns.Contains("DonVi"))
					item.TenDonVi = row["DonVi"].ToString();
				else if (dt.Columns.Contains("TenDonVi"))
					item.TenDonVi = row["TenDonVi"].ToString();

				// Cấp hành chính
				if (dt.Columns.Contains("Cap"))
					item.CapHanhChinh = row["Cap"].ToString();

				// Loại thiên tai
				if (dt.Columns.Contains("LoaiThienTai"))
					item.TenLoaiThienTai = row["LoaiThienTai"].ToString();
				else if (dt.Columns.Contains("TenLoaiThienTai"))
					item.TenLoaiThienTai = row["TenLoaiThienTai"].ToString();

				// Ghi chú
				if (dt.Columns.Contains("GhiChu") && row["GhiChu"] != DBNull.Value)
				{
					item.Ten = row["GhiChu"].ToString(); // Hoặc gán vào thuộc tính GhiChu của model nếu có
				}

				// Xử lý trạng thái File
				bool hasFile = FileAttachmentHelper.HasAttachment(item.Id);
				item.TrangThaiFile = hasFile ? "Có đính kèm" : "Không";

				listResult.Add(item);
			}

			return listResult;
		}

		// 3. Thêm mới
		public bool AddDisaster(int donViId, int loaiThienTaiId, int mucDo, string ghiChu, string filePath)
		{
			try
			{
				string query = @"INSERT INTO DiemThienTai (DonViId, LoaiThienTaiId, MucDo, GhiChu, NgayGhiNhan) 
                                 VALUES (@Dv, @Loai, @Muc, @Note, GETDATE());
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

				SqlParameter[] param = {
					new SqlParameter("@Dv", donViId),
					new SqlParameter("@Loai", loaiThienTaiId),
					new SqlParameter("@Muc", mucDo),
					new SqlParameter("@Note", ghiChu ?? (object)DBNull.Value)
				};

				object result = _dbHelper.ExecuteScalar(query, param);

				if (result != null && int.TryParse(result.ToString(), out int newId))
				{
					if (!string.IsNullOrEmpty(filePath))
					{
						FileAttachmentHelper.SaveFile(newId, filePath);
					}
					return true;
				}
				return false;
			}
			catch { return false; }
		}

		// 4. Cập nhật
		public bool UpdateDisaster(int id, int donViId, int loaiThienTaiId, int mucDo, string ghiChu, string filePath)
		{
			try
			{
				string query = @"UPDATE DiemThienTai 
                                 SET DonViId = @Dv, LoaiThienTaiId = @Loai, MucDo = @Muc, GhiChu = @Note 
                                 WHERE Id = @Id";

				SqlParameter[] param = {
					new SqlParameter("@Dv", donViId),
					new SqlParameter("@Loai", loaiThienTaiId),
					new SqlParameter("@Muc", mucDo),
					new SqlParameter("@Note", ghiChu ?? (object)DBNull.Value),
					new SqlParameter("@Id", id)
				};

				if (_dbHelper.ExecuteNonQuery(query, param) > 0)
				{
					if (!string.IsNullOrEmpty(filePath))
					{
						FileAttachmentHelper.SaveFile(id, filePath);
					}
					return true;
				}
				return false;
			}
			catch { return false; }
		}

		// 5. Xóa
		public bool DeleteDisaster(int id)
		{
			try
			{
				string query = "DELETE FROM DiemThienTai WHERE Id = @Id";
				bool dbDeleted = _dbHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@Id", id) }) > 0;

				if (dbDeleted)
				{
					FileAttachmentHelper.DeleteFile(id);
					return true;
				}
				return false;
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