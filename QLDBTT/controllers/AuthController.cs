using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class AuthController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
		private readonly ReportController _reportController = new ReportController();

		// 1. Đăng nhập
		public TaiKhoan Login(string username, string password)
		{
			string query = "SELECT * FROM ViewHoSo WHERE TenDangNhap = @User AND MatKhau = @Pass";
			SqlParameter[] param = {
				new SqlParameter("@User", SqlDbType.VarChar) { Value = username },
				new SqlParameter("@Pass", SqlDbType.VarChar) { Value = password }
			};

			DataTable dt = _dbHelper.ExecuteQuery(query, param);
			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				return new TaiKhoan
				{
					TenDangNhap = row["TenDangNhap"].ToString(),
					MatKhau = row["MatKhau"].ToString(),
					HoTen = row["HoTen"].ToString(), 
					QuyenId = Convert.ToInt32(row["QuyenId"]),
					TenQuyen = row["TenQuyen"].ToString(), 
					Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "",
					HoSoId = row["Id"] != DBNull.Value ? (int?)Convert.ToInt32(row["Id"]) : null
				};
			}
			return null;
		}

		// 2. Đổi mật khẩu (Tích hợp Log)
		public bool ChangePassword(string username, string oldPass, string newPass)
		{
			string queryCheck = "SELECT Count(*) FROM TaiKhoan WHERE Ten = @User AND MatKhau = @Old";
			SqlParameter[] paramCheck = {
				new SqlParameter("@User", username),
				new SqlParameter("@Old", oldPass)
			};

			int count = Convert.ToInt32(_dbHelper.ExecuteScalar(queryCheck, paramCheck));
			if (count == 0) return false;

			string queryUpdate = "UPDATE TaiKhoan SET MatKhau = @New WHERE Ten = @User";
			SqlParameter[] paramUpdate = {
				new SqlParameter("@New", newPass),
				new SqlParameter("@User", username)
			};

			bool result = _dbHelper.ExecuteNonQuery(queryUpdate, paramUpdate) > 0;
			if (result)
			{
				_reportController.LogAction(username, "Đổi mật khẩu", "Mật khẩu cũ", "Mật khẩu mới");
			}
			return result;
		}

		// 3. Quên mật khẩu/Reset mật khẩu (Tích hợp Log)
		public bool ResetPassword(string username, string email)
		{
			string queryCheck = @"SELECT Count(*) FROM ViewHoSo 
                                 WHERE TenDangNhap = @User AND Email = @Email";

			SqlParameter[] paramCheck = {
				new SqlParameter("@User", username),
				new SqlParameter("@Email", email)
			};

			int count = Convert.ToInt32(_dbHelper.ExecuteScalar(queryCheck, paramCheck));
			if (count == 0) return false;

			string defaultPass = "123456";
			string queryUpdate = "UPDATE TaiKhoan SET MatKhau = @Pass WHERE Ten = @User";
			bool result = _dbHelper.ExecuteNonQuery(queryUpdate, new SqlParameter[] {
				new SqlParameter("@Pass", defaultPass),
				new SqlParameter("@User", username)
			}) > 0;

			if (result)
			{
				_reportController.LogAction(username, "Khôi phục mật khẩu", "", "Reset về 123456");
			}
			return result;
		}

		// 4. Cập nhật hồ sơ cá nhân
		public bool UpdateUserFull(string username, string hoTen, string email, string sdt, int quyenId, string userThucHien)
		{
			try
			{
				// 1. Cập nhật Quyền trong bảng TaiKhoan (Sử dụng cột 'Ten' làm khóa)
				string queryTK = "UPDATE TaiKhoan SET QuyenId = @Quyen WHERE Ten = @User";
				_dbHelper.ExecuteNonQuery(queryTK, new[] {
			new SqlParameter("@Quyen", quyenId),
			new SqlParameter("@User", username)
		});

				// 2. Tìm HoSoId liên kết với tài khoản này để cập nhật bảng HoSo
				string queryGetId = "SELECT HoSoId FROM TaiKhoan WHERE Ten = @User";
				object result = _dbHelper.ExecuteScalar(queryGetId, new[] { new SqlParameter("@User", username) });

				if (result != null && result != DBNull.Value)
				{
					int hoSoId = Convert.ToInt32(result);

					// 3. Cập nhật thông tin chi tiết trong bảng HoSo
					string queryHS = "UPDATE HoSo SET Ten = @Ten, Email = @Email, SDT = @SDT WHERE Id = @Id";
					_dbHelper.ExecuteNonQuery(queryHS, new[] {
				new SqlParameter("@Ten", hoTen),
				new SqlParameter("@Email", (object)email ?? DBNull.Value),
				new SqlParameter("@SDT", (object)sdt ?? DBNull.Value),
				new SqlParameter("@Id", hoSoId)
			});
				}

				// 4. Ghi nhật ký hệ thống
				_reportController.LogAction(userThucHien, "Cập nhật người dùng", username, $"Tên: {hoTen}, QuyềnId: {quyenId}");

				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Lỗi UpdateUserFull: " + ex.Message);
				return false;
			}
		}
	}
}