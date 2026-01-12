using System;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class SystemLogsView : UserControl
	{
		private readonly ReportController _controller = new ReportController();

		public SystemLogsView()
		{
			InitializeComponent();

			// Mặc định xem 7 ngày gần nhất
			dpFrom.SelectedDate = DateTime.Now.AddDays(-7);
			dpTo.SelectedDate = DateTime.Now;

			LoadLogs();
		}

		private void LoadLogs()
		{
			try
			{
				DateTime from = dpFrom.SelectedDate ?? DateTime.Now.AddDays(-30);
				DateTime to = dpTo.SelectedDate ?? DateTime.Now;
				to = to.Date.AddDays(1).AddSeconds(-1);

				string searchUser = txtUser.Text.Trim();

				// Lấy tên người dùng đang đăng nhập hiện tại
				string currentLoggedUser = Helpers.UserSession.CurrentUser?.TenDangNhap;

				// Truyền thêm currentLoggedUser vào để lọc nhật ký đổi mật khẩu
				dgLogs.ItemsSource = _controller.GetLogs(from, to, currentLoggedUser, searchUser);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải nhật ký: " + ex.Message);
			}
		}

		private void btnFilter_Click(object sender, RoutedEventArgs e)
		{
			LoadLogs();
		}
	}
}