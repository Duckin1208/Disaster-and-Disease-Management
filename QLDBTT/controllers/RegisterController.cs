using System;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Controllers
{
	public class RegisterController
	{
		private readonly DatabaseHelper _dbHelper;

		public RegisterController()
		{
			_dbHelper = new DatabaseHelper();
		}

		
		public bool Register(string username, string fullName, string email, string password, string phone)
		{
			try
			{
				string checkQuery = "SELECT COUNT(*) FROM TaiKhoan WHERE Ten = @User";
				var checkParams = new[] {
					new SqlParameter("@User", SqlDbType.VarChar) { Value = username }
				};

				int count = Convert.ToInt32(_dbHelper.ExecuteScalar(checkQuery, checkParams));
				if (count > 0) return false;

				string insertHoSoQuery = @"INSERT INTO HoSo (Ten, Email, SDT) VALUES (@Ten, @Email, @SDT);
                                           SELECT CAST(SCOPE_IDENTITY() as int);";

				var hoSoParams = new[] {
					new SqlParameter("@Ten", fullName),
					new SqlParameter("@Email", (object)email ?? DBNull.Value),
					new SqlParameter("@SDT", (object)phone ?? DBNull.Value) 
                };

				object resultId = _dbHelper.ExecuteScalar(insertHoSoQuery, hoSoParams);
				if (resultId == null) return false;
				int hoSoId = Convert.ToInt32(resultId);

				// 3. Thêm mới vào bảng TaiKhoan
				string insertAccountQuery = "INSERT INTO TaiKhoan (Ten, MatKhau, QuyenId, HoSoId) VALUES (@User, @Pass, @Role, @ProfileId)";

				var accountParams = new[] {
					new SqlParameter("@User", username),
					new SqlParameter("@Pass", password),
					new SqlParameter("@Role", 3), // Mặc định Staff
                    new SqlParameter("@ProfileId", hoSoId)
				};

				return _dbHelper.ExecuteNonQuery(insertAccountQuery, accountParams) > 0;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Lỗi Register: " + ex.Message);
				return false;
			}
		}
	}
}