using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class ReportController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		// Lấy nhật ký hệ thống
		public List<NhatKyHeThong> GetLogs(DateTime from, DateTime to, string currentUser, string searchUser = "")
		{
			// Điều kiện logic: 
			// 1. Hiện tất cả các thao tác KHÔNG PHẢI là 'Đổi mật khẩu'
			// 2. Chỉ hiện thao tác 'Đổi mật khẩu' NẾU tài khoản đó trùng với người đang xem
			string query = @"SELECT * FROM ViewLichSuThaoTac 
                     WHERE (ThoiGianDangNhap BETWEEN @From AND @To)
                     AND (ThaoTac <> N'Đổi mật khẩu' OR TaiKhoanTen = @Current) ";

			List<SqlParameter> param = new List<SqlParameter> {
		new SqlParameter("@From", from),
		new SqlParameter("@To", to),
		new SqlParameter("@Current", currentUser) // Người đang đăng nhập
    };

			if (!string.IsNullOrEmpty(searchUser))
			{
				query += " AND TaiKhoanTen LIKE @User";
				param.Add(new SqlParameter("@User", "%" + searchUser + "%"));
			}

			query += " ORDER BY ThoiGianDangNhap DESC";

			var list = new List<NhatKyHeThong>();
			DataTable dt = _dbHelper.ExecuteQuery(query, param.ToArray());

			foreach (DataRow row in dt.Rows)
			{
				list.Add(new NhatKyHeThong
				{
					Id = Convert.ToInt32(row["Id"]),
					TaiKhoan = row["TaiKhoanTen"].ToString(),
					HoTen = row["HoTen"].ToString(),
					HanhDong = row["ThaoTac"].ToString(),
					// Nếu là nhật ký đổi mật khẩu của mình, có thể ẩn giá trị mật khẩu để bảo mật hơn
					NoiDung = row["ThaoTac"].ToString() == "Đổi mật khẩu"
							  ? "Đã thực hiện thay đổi mật khẩu cá nhân"
							  : $"Cũ: {row["GiaTriCu"]} -> Mới: {row["GiaTriMoi"]}",
					ThoiGian = Convert.ToDateTime(row["ThoiGianDangNhap"])
				});
			}
			return list;
		}

		// Dashboard Thống kê
		public ThongKeTongHop GetOverview()
		{
			var stats = new ThongKeTongHop();
			stats.TongNguoiDung = Convert.ToInt32(_dbHelper.ExecuteScalar("SELECT COUNT(*) FROM TaiKhoan"));
			stats.TongTacDong = Convert.ToInt32(_dbHelper.ExecuteScalar("SELECT COUNT(*) FROM LichSuThaoTac"));
			stats.TongTruyCap = stats.TongTacDong; // Tạm thời lấy số log làm số truy cập
			return stats;
		}

		// Hàm ghi Log dùng chung
		public void LogAction(string user, string action, string oldVal, string newVal)
		{
			string query = @"INSERT INTO LichSuThaoTac (TaiKhoanTen, ThaoTac, GiaTriCu, GiaTriMoi, ThoiGianDangNhap)
                             VALUES (@User, @Act, @Old, @New, GETDATE())";

			_dbHelper.ExecuteNonQuery(query, new[] {
				new SqlParameter("@User", user),
				new SqlParameter("@Act", action),
				new SqlParameter("@Old", oldVal ?? ""),
				new SqlParameter("@New", newVal ?? "")
			});
		}
	}
}