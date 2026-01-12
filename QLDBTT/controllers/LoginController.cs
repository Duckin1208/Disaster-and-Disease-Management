using System;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class LoginController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		public TaiKhoan Login(string username, string password)
		{
			// Sử dụng ViewHoSo với các cột alias: TenDangNhap, HoTen
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
					Id = Convert.ToInt32(row["Id"]),
					HoTen = row["HoTen"].ToString(),
					TenDangNhap = row["TenDangNhap"].ToString(),
					QuyenId = Convert.ToInt32(row["QuyenId"]),
					TenQuyen = row["TenQuyen"].ToString(),
					Email = row["Email"]?.ToString(),
					SDT = row["SDT"]?.ToString()
				};
			}
			return null;
		}
	}
}