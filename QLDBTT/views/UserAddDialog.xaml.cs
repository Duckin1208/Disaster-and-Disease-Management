using System;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class UserAddDialog : Window
	{
		private readonly UserManagementController _userController = new UserManagementController();
		private string _userThucHien;

		// Constructor nhận tên người đang đăng nhập để ghi log
		public UserAddDialog(string currentUser)
		{
			InitializeComponent();
			_userThucHien = currentUser;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra dữ liệu bắt buộc
			if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
				string.IsNullOrWhiteSpace(txtPassword.Password) ||
				string.IsNullOrWhiteSpace(txtPhone.Text))
			{
				MessageBox.Show("Vui lòng nhập Tên đăng nhập, Mật khẩu và Số điện thoại!", "Cảnh báo");
				return;
			}

			try
			{
				int quyenId = 3;
				if (cbRole.SelectedItem is ComboBoxItem item)
					quyenId = int.Parse(item.Tag.ToString());

				// Gọi Controller với đầy đủ tham số (bao gồm SDT và người thực hiện)
				bool result = _userController.AddUser(
					txtUsername.Text.Trim(),
					txtPassword.Password,
					quyenId,
					txtFullName.Text.Trim(),
					txtEmail.Text.Trim(),
					txtPhone.Text.Trim(), // Lấy từ ô mới thêm
					_userThucHien        // Tham số thứ 7 để ghi nhật ký
				);

				if (result)
				{
					MessageBox.Show("Thêm tài khoản thành công!");
					this.DialogResult = true;
					this.Close();
				}
				else
				{
					MessageBox.Show("Lỗi: Tên đăng nhập có thể đã tồn tại.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi hệ thống: " + ex.Message);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
	}
}