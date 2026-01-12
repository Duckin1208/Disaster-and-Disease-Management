using System;
using System.Windows;
using System.Windows.Media;
using Disease_Disaster.Models;
using Disease_Disaster.Views;
using Disease_Disaster.Helpers;

namespace Disease_Disaster
{
	public partial class MainWindow : Window
	{
		private readonly TaiKhoan _currentUser;

		public MainWindow(TaiKhoan user)
		{
			InitializeComponent();
			_currentUser = user;

			// 1. Hiển thị thông tin người dùng
			if (txtUserFullName != null) txtUserFullName.Text = user.HoTen;
			if (txtUserRole != null) txtUserRole.Text = $"Vai trò: {user.TenQuyen}";

			ApplyPermissions();

			// 2. Mặc định vào trang Thiên tai khi khởi động
			NavDisaster_Click(null, null);
		}

		// --- PHÂN QUYỀN GIAO DIỆN ---
		private void ApplyPermissions()
		{
			if (_currentUser == null) return;

			// Nếu là Staff (QuyềnId = 3)
			if (_currentUser.QuyenId == 3)
			{
				if (btnUserMgmt != null) btnUserMgmt.Content = "👤  Thông tin cá nhân";
				if (btnLogs != null) btnLogs.Visibility = Visibility.Collapsed;
			}
		}

		//Đổi mật khẩu
		private void btnChangePass_Click(object sender, RoutedEventArgs e)
		{
			if (_currentUser != null)
			{
				// Mở cửa sổ đổi mật khẩu và truyền TenDangNhap (đã alias từ Ten trong SQL)
				ChangePasswordWindow changeWin = new ChangePasswordWindow(_currentUser.TenDangNhap);
				changeWin.Owner = this; // Căn giữa theo MainWindow
				changeWin.ShowDialog();
			}
		}

		// --- NAVIGATION (ĐIỀU HƯỚNG) ---
		private void NavUser_Click(object sender, RoutedEventArgs e)
		{
			if (_currentUser.QuyenId == 3) txtTitle.Text = "Thông tin cá nhân";
			else txtTitle.Text = "Quản lý Người dùng";

			MainContentFrame.Content = new UserManagementView();
		}

		private void NavAdmin_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Đơn vị Hành chính";
			MainContentFrame.Content = new AdministrativeView();
		}

		private void NavDisaster_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Quản lý Thiên tai & Báo cáo";
			MainContentFrame.Content = new DisasterManagementView();
		}

		private void NavDisease_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Quản lý Dịch bệnh & Tiêm phòng";
			MainContentFrame.Content = new DiseaseManagementView();
		}

		private void NavVet_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Quản lý Cơ sở Chăn nuôi & Thú y";
			MainContentFrame.Content = new VeterinaryFacilityView();
		}

		private void NavChiCuc_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Quản lý Chi Cục Thú Y & Địa Bàn";
			MainContentFrame.Content = new ChiCucManagementView();
		}

		private void NavSafeZone_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Tra cứu Vùng Chăn nuôi An toàn Dịch bệnh";
			MainContentFrame.Content = new Views.SafeZoneView();
		}

		private void NavLogs_Click(object sender, RoutedEventArgs e)
		{
			txtTitle.Text = "Nhật ký hệ thống";
			MainContentFrame.Content = new SystemLogsView();
		}

		//Hệ thống
		private void btnLogout_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				if (UserSession.IsLoggedIn) UserSession.Clear();

				// Quay lại màn hình đăng nhập
				LoginWindow login = new LoginWindow();
				login.Show();
				this.Close();
			}
		}

		// Cập nhật trạng thái Online/Offline trực quan
		public void SetStatus(bool isOnline)
		{
			var converter = new BrushConverter();
			var colorCode = isOnline ? "#2ECC71" : "#95A5A6";
			var statusText = isOnline ? "Đang hoạt động" : "Không hoạt động";

			Brush statusBrush = (Brush)converter.ConvertFrom(colorCode);

			if (statusDot != null) statusDot.Fill = statusBrush;
			if (txtStatus != null)
			{
				txtStatus.Text = statusText;
				txtStatus.Foreground = statusBrush;
			}
		}
	}
}