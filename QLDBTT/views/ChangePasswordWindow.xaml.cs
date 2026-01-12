using System.Windows;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Views
{
	public partial class ChangePasswordWindow : Window
	{
		private readonly AuthController _authController = new AuthController();
		private readonly string _username;

		public ChangePasswordWindow(string username)
		{
			InitializeComponent();
			_username = username;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			string oldPass = txtOldPass.Password;
			string newPass = txtNewPass.Password;
			string confirmPass = txtConfirmPass.Password;

			// 1. Kiểm tra dữ liệu đầu vào
			if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
				return;
			}

			if (newPass != confirmPass)
			{
				MessageBox.Show("Xác nhận mật khẩu không khớp!");
				return;
			}

			// 2. Gọi Controller thực hiện đổi mật khẩu
			if (_authController.ChangePassword(_username, oldPass, newPass))
			{
				MessageBox.Show("Đổi mật khẩu thành công!");
				this.Close();
			}
			else
			{
				MessageBox.Show("Mật khẩu cũ không chính xác!");
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}