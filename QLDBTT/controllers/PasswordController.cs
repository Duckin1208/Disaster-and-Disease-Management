using System;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Controllers
{
	public class PasswordController
	{
		private readonly DatabaseHelper _dbHelper;

		public PasswordController()
		{
			_dbHelper = new DatabaseHelper();
		}

		// ĐỔI MẬT KHẨU
		public bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			try
			{
				// 1. Kiểm tra mật khẩu cũ
				string queryCheck = "SELECT MatKhau FROM TaiKhoan WHERE TenDangNhap = @User";
				DataTable dt = _dbHelper.ExecuteQuery(queryCheck, new[] { new SqlParameter("@User", username) });

				if (dt.Rows.Count == 0) return false; // Không tìm thấy user

				string currentDbPassword = dt.Rows[0]["MatKhau"].ToString();

				// So sánh (Lưu ý: Nếu bạn có dùng MD5 thì phải mã hóa oldPassword trước khi so sánh)
				if (currentDbPassword != oldPassword) return false;

				// 2. Cập nhật mật khẩu mới
				string queryUpdate = "UPDATE TaiKhoan SET MatKhau = @NewPass WHERE TenDangNhap = @User";
				return _dbHelper.ExecuteNonQuery(queryUpdate, new[] {
					new SqlParameter("@NewPass", newPassword),
					new SqlParameter("@User", username)
				}) > 0;
			}
			catch (Exception)
			{
				return false;
			}
		}

		// QUÊN MẬT KHẨU 
		public bool ResetPasswordByEmail(string username, string email, string newDefaultPassword)
		{
			try
			{
				// 1. Kiểm tra xem Username có khớp với Email đăng ký không (Bảng ViewHoSo hoặc Join TaiKhoan-HoSo)
				// Giả sử dùng ViewHoSo có đủ cột TenDangNhap và Email
				string queryCheck = "SELECT COUNT(*) FROM ViewHoSo WHERE TenDangNhap = @User AND Email = @Email";

				int count = (int)_dbHelper.ExecuteScalar(queryCheck, new[] {
					new SqlParameter("@User", username),
					new SqlParameter("@Email", email)
				});

				if (count == 0) return false; // Thông tin không khớp

				// 2. Reset mật khẩu về mặc định (Ví dụ: 123456)
				string queryUpdate = "UPDATE TaiKhoan SET MatKhau = @NewPass WHERE TenDangNhap = @User";
				return _dbHelper.ExecuteNonQuery(queryUpdate, new[] {
					new SqlParameter("@NewPass", newDefaultPassword),
					new SqlParameter("@User", username)
				}) > 0;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}