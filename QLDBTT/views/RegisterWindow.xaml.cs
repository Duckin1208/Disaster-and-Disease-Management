using System;
using System.Windows;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class RegisterWindow : Window
	{
		private readonly RegisterController _registerController = new RegisterController();

		public RegisterWindow()
		{
			InitializeComponent();
		}

		private void btnRegister_Click(object sender, RoutedEventArgs e)
		{
			// 1. Lấy dữ liệu từ giao diện
			string fullName = txtFullName.Text.Trim();
			string email = txtEmail.Text.Trim();
			string phone = txtPhone.Text.Trim();
			string username = txtUsername.Text.Trim();
			string password = txtPassword.Password;

			// 2. Kiểm tra tính hợp lệ cơ bản
			if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) ||
				string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ các thông tin có dấu (*)", "Thông báo");
				return;
			}

			// 3. Gọi Controller để thực hiện đăng ký
			try
			{
				bool isSuccess = _registerController.Register(username, fullName, email, password, phone);

				if (isSuccess)
				{
					MessageBox.Show("Chúc mừng! Bạn đã đăng ký tài khoản thành công.", "Thông báo");
					// Quay lại màn hình đăng nhập
					btnBack_Click(null, null);
				}
				else
				{
					MessageBox.Show("Đăng ký thất bại. Tên đăng nhập có thể đã tồn tại trong hệ thống.", "Lỗi");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi hệ thống: " + ex.Message);
			}
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			// Mở lại LoginWindow và đóng cửa sổ hiện tại
			LoginWindow login = new LoginWindow();
			login.Show();
			this.Close();
		}
	}
}