using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class ChiCucController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		// ==========================================
		// PHẦN 1: QUẢN LÝ THÔNG TIN CHI CỤC (CRUD)
		// ==========================================

		// 1. Lấy danh sách (Trả về DataTable để bind vào DataGrid dễ dàng)
		public DataTable GetAllChiCuc()
		{
			return _dbHelper.ExecuteQuery("SELECT * FROM ChiCucThuy");
		}

		// 2. Tìm kiếm (Trả về List Object nếu cần xử lý logic)
		public List<ChiCucThuy> Search(string keyword = "")
		{
			string query = "SELECT * FROM ChiCucThuy WHERE Ten LIKE @Key";
			var list = new List<ChiCucThuy>();
			DataTable dt = _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Key", "%" + keyword + "%") });

			foreach (DataRow row in dt.Rows)
			{
				list.Add(new ChiCucThuy
				{
					Id = (int)row["Id"],
					Ten = row["Ten"].ToString()
				});
			}
			return list;
		}

		public bool Add(string ten)
		{
			return _dbHelper.ExecuteNonQuery("INSERT INTO ChiCucThuy (Ten) VALUES (@Ten)",
				new[] { new SqlParameter("@Ten", ten) }) > 0;
		}

		public bool Update(int id, string ten)
		{
			return _dbHelper.ExecuteNonQuery("UPDATE ChiCucThuy SET Ten = @Ten WHERE Id = @Id",
				new[] { new SqlParameter("@Ten", ten), new SqlParameter("@Id", id) }) > 0;
		}

		// Xóa Chi cục (Cần set NULL các tỉnh đang thuộc chi cục này trước khi xóa để tránh lỗi khóa ngoại)
		public bool Delete(int id)
		{
			try
			{
				// Bước 1: Gỡ các tỉnh ra khỏi chi cục này trước
				string updateQuery = "UPDATE DonVi SET ChiCucThuyId = NULL WHERE ChiCucThuyId = @Id";
				_dbHelper.ExecuteNonQuery(updateQuery, new[] { new SqlParameter("@Id", id) });

				// Bước 2: Xóa Chi cục
				string deleteQuery = "DELETE FROM ChiCucThuy WHERE Id = @Id";
				return _dbHelper.ExecuteNonQuery(deleteQuery, new[] { new SqlParameter("@Id", id) }) > 0;
			}
			catch
			{
				return false;
			}
		}

		// ==========================================
		// PHẦN 2: QUẢN LÝ ĐỊA BÀN (TỈNH/THÀNH)
		// (Dùng cho giao diện ChiCucManagementView)
		// ==========================================

		// Lấy danh sách Tỉnh/Thành thuộc 1 Chi cục cụ thể
		public DataTable GetProvincesByChiCuc(int chiCucId)
		{
			// Chỉ lấy đơn vị cấp 1 (Tỉnh/Thành phố) -> HanhChinhId = 1
			string query = @"SELECT Id, Ten, TenHanhChinh 
                             FROM DonVi 
                             WHERE ChiCucThuyId = @Id AND HanhChinhId = 1
                             ORDER BY Ten";
			return _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Id", chiCucId) });
		}

		// Lấy danh sách Tỉnh/Thành CHƯA có Chi cục quản lý (để nạp vào ComboBox thêm mới)
		public DataTable GetAvailableProvinces()
		{
			string query = @"SELECT Id, Ten 
                             FROM DonVi 
                             WHERE ChiCucThuyId IS NULL AND HanhChinhId = 1
                             ORDER BY Ten";
			return _dbHelper.ExecuteQuery(query);
		}

		// Cập nhật Chi cục cho Tỉnh (Dùng cho cả Thêm và Xóa)
		// Nếu thêm: chiCucId = ID chi cục
		// Nếu xóa: chiCucId = NULL
		public bool UpdateProvinceChiCuc(int donViId, int? chiCucId)
		{
			try
			{
				string query = "UPDATE DonVi SET ChiCucThuyId = @CId WHERE Id = @DId";

				SqlParameter[] param = {
					new SqlParameter("@DId", donViId),
					new SqlParameter("@CId", chiCucId.HasValue ? (object)chiCucId.Value : DBNull.Value)
				};

				return _dbHelper.ExecuteNonQuery(query, param) > 0;
			}
			catch { return false; }
		}
	}
}