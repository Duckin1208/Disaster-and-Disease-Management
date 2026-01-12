using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class UserManagementController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
		private readonly ReportController _reportController = new ReportController();

		// 1. Lấy danh sách người dùng
		public List<NguoiDungFull> GetUsers(string keyword = "", int? filterRoleId = null)
		{
			string query = @"SELECT * FROM ViewHoSo 
                             WHERE (TenDangNhap LIKE @Key OR HoTen LIKE @Key OR Email LIKE @Key)";

			var param = new List<SqlParameter> {
				new SqlParameter("@Key", "%" + keyword + "%")
			};

			if (filterRoleId.HasValue)
			{
				query += " AND QuyenId = @RoleId";
				param.Add(new SqlParameter("@RoleId", filterRoleId.Value));
			}
			query += " ORDER BY QuyenId ASC, TenDangNhap ASC";

			var list = new List<NguoiDungFull>();
			DataTable dt = _dbHelper.ExecuteQuery(query, param.ToArray());

			foreach (DataRow row in dt.Rows)
			{
				list.Add(new NguoiDungFull
				{
					TenDangNhap = row["TenDangNhap"]?.ToString(),
					HoTen = row["HoTen"]?.ToString(),
					Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "",
					SDT = row["SDT"] != DBNull.Value ? row["SDT"].ToString() : "",
					QuyenId = row["QuyenId"] != DBNull.Value ? Convert.ToInt32(row["QuyenId"]) : 0,
					TenQuyen = row["TenQuyen"]?.ToString(),
					HoSoId = row["Id"] != DBNull.Value ? (int?)Convert.ToInt32(row["Id"]) : null
				});
			}
			return list;
		}

		// 2. Thêm người dùng (Tích hợp Log)
		public bool AddUser(string username, string password, int quyenId, string hoTen, string email, string sdt, string userThucHien)
		{
			try
			{
				// Bước 1: Thêm vào bảng HoSo (Lấy ID hồ sơ)
				string queryHoSo = "INSERT INTO HoSo (Ten, Email, SDT) VALUES (@Ten, @Email, @SDT); SELECT SCOPE_IDENTITY();";
				object hoSoIdObj = _dbHelper.ExecuteScalar(queryHoSo, new[] {
			new SqlParameter("@Ten", hoTen),
			new SqlParameter("@Email", (object)email ?? DBNull.Value),
			new SqlParameter("@SDT", (object)sdt ?? DBNull.Value) // SDT được lưu tại đây
        });

				if (hoSoIdObj == null) return false;
				int newHoSoId = Convert.ToInt32(hoSoIdObj);

				// Bước 2: Thêm vào bảng TaiKhoan (Cột Ten là Username)
				string queryTK = "INSERT INTO TaiKhoan (Ten, MatKhau, QuyenId, HoSoId) VALUES (@User, @Pass, @Quyen, @HSId)";
				bool result = _dbHelper.ExecuteNonQuery(queryTK, new[] {
			new SqlParameter("@User", username),
			new SqlParameter("@Pass", password),
			new SqlParameter("@Quyen", quyenId),
			new SqlParameter("@HSId", newHoSoId)
		}) > 0;

				if (result) _reportController.LogAction(userThucHien, "Thêm người dùng", "", $"Tài khoản: {username}, SDT: {sdt}");
				return result;
			}
			catch { return false; }
		}

		// 3. Xóa người dùng (Tích hợp Log)
		public bool DeleteUser(string username, string userThucHien)
		{
			try
			{
				string query = "DELETE FROM TaiKhoan WHERE Ten = @User";
				SqlParameter[] param = { new SqlParameter("@User", username) };

				bool result = _dbHelper.ExecuteNonQuery(query, param) > 0;
				if (result)
				{
					_reportController.LogAction(userThucHien, "Xóa người dùng", username, "");
				}
				return result;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Lỗi DeleteUser: " + ex.Message);
				return false;
			}
		}
		public bool UpdateUserFull(string username, string hoTen, string email, string sdt, int quyenId, string userThucHien)
		{
			try
			{
				// 1. Cập nhật Quyền hạn trong bảng TaiKhoan (Sử dụng cột 'Ten' là Username)
				string queryTK = "UPDATE TaiKhoan SET QuyenId = @Quyen WHERE Ten = @User";
				_dbHelper.ExecuteNonQuery(queryTK, new[] {
			new SqlParameter("@Quyen", quyenId),
			new SqlParameter("@User", username)
		});

				// 2. Lấy HoSoId từ tài khoản để cập nhật bảng HoSo
				string queryGetId = "SELECT HoSoId FROM TaiKhoan WHERE Ten = @User";
				object result = _dbHelper.ExecuteScalar(queryGetId, new[] { new SqlParameter("@User", username) });

				if (result != null && result != DBNull.Value)
				{
					int hoSoId = Convert.ToInt32(result);

					// 3. Cập nhật thông tin chi tiết: Họ tên, Email, SDT trong bảng HoSo
					string queryHS = "UPDATE HoSo SET Ten = @Ten, Email = @Email, SDT = @SDT WHERE Id = @Id";
					_dbHelper.ExecuteNonQuery(queryHS, new[] {
				new SqlParameter("@Ten", hoTen),
				new SqlParameter("@Email", (object)email ?? DBNull.Value),
				new SqlParameter("@SDT", (object)sdt ?? DBNull.Value),
				new SqlParameter("@Id", hoSoId)
			});
				}

				// 4. Ghi nhật ký hệ thống (Log)
				_reportController.LogAction(userThucHien, "Cập nhật người dùng", username, $"Tên: {hoTen}, Quyền: {quyenId}, SDT: {sdt}");

				return true;
			}
			catch (Exception ex)
			{
				// Ghi log lỗi vào Output để kiểm tra nếu cần
				System.Diagnostics.Debug.WriteLine("Lỗi UpdateUserFull: " + ex.Message);
				return false;
			}
		}
	}
}