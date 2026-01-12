using System;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Views
{
	public partial class UserEditDialog : Window
	{
		private readonly UserManagementController _controller = new UserManagementController();
		private readonly TaiKhoan _currentUser; // Người đang thực hiện thao tác
		private readonly NguoiDungFull _selectedUser; // Người được chọn để sửa

		public UserEditDialog(NguoiDungFull selectedUser, TaiKhoan currentUser)
		{
			InitializeComponent();
			_selectedUser = selectedUser;
			_currentUser = currentUser;

			// 1. Điền dữ liệu cũ vào các ô nhập liệu
			txtUsername.Text = selectedUser.TenDangNhap;
			txtFullName.Text = selectedUser.HoTen;
			txtEmail.Text = selectedUser.Email;
			txtPhone.Text = selectedUser.SDT;

			// 2. Chọn đúng Quyền hiện tại trong ComboBox
			foreach (ComboBoxItem item in cbRole.Items)
			{
				if (item.Tag.ToString() == selectedUser.QuyenId.ToString())
				{
					cbRole.SelectedItem = item;
					break;
				}
			}

			// 3. THỰC HIỆN PHÂN QUYỀN TRÊN GIAO DIỆN
			ApplyPermissions();
		}

		private void ApplyPermissions()
		{
			// Nếu người thực hiện là Staff (QuyenId > 2)
			if (_currentUser.QuyenId > 2)
			{
				// Không cho phép sửa quyền hạn của chính mình
				cbRole.IsEnabled = false;

				// Gợi ý: Có thể khóa thêm các trường nhạy cảm khác nếu cần
				// txtUsername.IsEnabled = false; (Đã IsReadOnly trong XAML)
			}
		}

		private void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra dữ liệu bắt buộc
			if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
			{
				MessageBox.Show("Họ tên và Số điện thoại không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				string username = txtUsername.Text;
				string fullName = txtFullName.Text.Trim();
				string email = txtEmail.Text.Trim();
				string phone = txtPhone.Text.Trim();

				int roleId = _selectedUser.QuyenId; 
				if (cbRole.SelectedItem is ComboBoxItem item && item.Tag != null)
				{
					roleId = int.Parse(item.Tag.ToString());
				}

				// Gọi hàm cập nhật từ Controller
				// Truyền _currentUser.TenDangNhap vào cuối để ghi nhật ký hệ thống
				bool success = _controller.UpdateUserFull(
					username,
					fullName,
					email,
					phone,
					roleId,
					_currentUser.TenDangNhap
				);

				if (success)
				{
					MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
					this.DialogResult = true;
					this.Close();
				}
				else
				{
					MessageBox.Show("Lỗi: Không thể cập nhật dữ liệu. Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi hệ thống: " + ex.Message);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}