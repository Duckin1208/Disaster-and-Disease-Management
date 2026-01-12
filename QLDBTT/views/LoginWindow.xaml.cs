using System.Windows;
using Disease_Disaster.Controllers; 
using Disease_Disaster.Models;      
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Views
{
	public partial class LoginWindow : Window
	{
		private readonly LoginController _loginController = new LoginController();

		public LoginWindow()
		{
			InitializeComponent();
		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			// Gọi Controller thực hiện đăng nhập
			var account = _loginController.Login(txtUsername.Text, txtPassword.Password);

			if (account != null)
			{
				// LƯU PHIÊN ĐĂNG NHẬP: Gán vào lớp UserSession bạn đã tạo
				Disease_Disaster.Helpers.UserSession.CurrentUser = account;

				// Mở màn hình chính và truyền đối tượng user
				MainWindow main = new MainWindow(account);
				main.Show();
				this.Close();
			}
			else
			{
				MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// PHƯƠNG THỨC MỞ CỬA SỔ ĐĂNG KÝ 
		private void btnRegister_Click(object sender, RoutedEventArgs e)
		{
			RegisterWindow regWindow = new RegisterWindow();
			regWindow.ShowDialog(); // Hiển thị cửa sổ đăng ký
		}

		private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Hệ thống sẽ reset mật khẩu về 123456 nếu thông tin Email khớp.");
		}

		private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
	}
}