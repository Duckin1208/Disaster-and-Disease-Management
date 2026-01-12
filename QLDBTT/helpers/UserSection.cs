using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disease_Disaster.Models; 

namespace Disease_Disaster.Helpers
{
	public static class UserSession
	{
		// Thay vì lưu string trong Resources, ta lưu object TaiKhoan vào biến static
		// Để có thể truy cập UserSession.CurrentUser từ bất cứ đâu
		private static TaiKhoan _currentUser;

		public static TaiKhoan CurrentUser
		{
			get { return _currentUser; }
			set { _currentUser = value; }
		}

		// Helper: Kiểm tra xem có người dùng đăng nhập chưa
		public static bool IsLoggedIn
		{
			get { return _currentUser != null; }
		}

		// Helper: Kiểm tra nhanh xem có phải Admin không (Dựa trên dữ liệu bảng Quyen trong SQL)
		public static bool IsAdmin
		{
			get
			{
				// Kiểm tra null và so sánh tên quyền (Theo SQL insert: N'Quản trị hệ thống')
				return _currentUser != null &&
					   (_currentUser.TenQuyen == "Quản trị hệ thống" || _currentUser.TenQuyen == "Admin");
			}
		}

		// Hàm đăng xuất (Xóa session)
		public static void Clear()
		{
			_currentUser = null;
		}
	}
}