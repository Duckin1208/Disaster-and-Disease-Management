using System;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Views
{
	public partial class UserManagementView : UserControl
	{
		private readonly UserManagementController _controller = new UserManagementController();
		private readonly TaiKhoan _currentUser;

		public UserManagementView()
		{
			InitializeComponent();
			// Lấy thông tin người dùng hiện tại từ Session
			_currentUser = UserSession.CurrentUser;

			ApplySecurity();
			LoadData();
		}

		private void ApplySecurity()
		{
			if (_currentUser == null) return;

			// Admin (2) hoặc Dev (1) mới có quyền thao tác (Thêm/Xóa)
			bool isAdmin = (_currentUser.QuyenId == 1 || _currentUser.QuyenId == 2);

			if (pnlAdminActions != null)
				pnlAdminActions.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

			// Ẩn/Hiện cột tác vụ (Xóa) trong DataGrid
			if (dgUsers.Columns.Count > 0)
			{
				// Giả sử cột cuối cùng hoặc cột có tên colAction là cột Tác vụ
				// Bạn nên đặt x:Name="colAction" cho DataGridTemplateColumn trong XAML
				// colAction.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void LoadData()
		{
			if (_currentUser == null) return;

			string keyword = txtSearch.Text.Trim();

			// Phân quyền xem danh sách
			if (_currentUser.QuyenId == 3) // Staff (Cán bộ)
			{
				// Chỉ hiện danh sách những người có quyền Staff (Quyền 3)
				dgUsers.ItemsSource = _controller.GetUsers(keyword, filterRoleId: 3);
			}
			else // Admin/Dev
			{
				// Hiện tất cả người dùng
				dgUsers.ItemsSource = _controller.GetUsers(keyword, filterRoleId: null);
			}
		}

		private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			LoadData();
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadData();
		}
		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			// 1. Lấy thông tin người dùng được chọn từ DataGrid
			if (dgUsers.SelectedItem is NguoiDungFull selectedUser)
			{
				bool isAdmin = (_currentUser.QuyenId == 1 || _currentUser.QuyenId == 2);
				bool isOwner = (selectedUser.TenDangNhap == _currentUser.TenDangNhap);

				// 2. Kiểm tra quyền sửa
				if (isAdmin || isOwner)
				{
					UserEditDialog editDialog = new UserEditDialog(selectedUser, _currentUser);
					editDialog.Owner = Window.GetWindow(this);

					if (editDialog.ShowDialog() == true)
					{
						LoadData();
					}
				}
				else
				{
					MessageBox.Show("Bạn không có quyền sửa thông tin của người khác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (_currentUser.QuyenId > 2)
			{
				MessageBox.Show("Bạn không có quyền thực hiện chức năng này!");
				return;
			}

			// Ép kiểu dòng đang chọn về Model NguoiDungFull
			if (dgUsers.SelectedItem is NguoiDungFull user)
			{
				// Không cho phép tự xóa chính mình
				if (user.TenDangNhap == _currentUser.TenDangNhap)
				{
					MessageBox.Show("Không thể xóa tài khoản đang đăng nhập!");
					return;
				}

				if (MessageBox.Show($"Xóa người dùng {user.TenDangNhap}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					
					// Tham số thứ 2 là tên người thực hiện để ghi Log
					if (_controller.DeleteUser(user.TenDangNhap, _currentUser.TenDangNhap))
					{
						MessageBox.Show("Đã xóa thành công.");
						LoadData();
					}
					else
					{
						MessageBox.Show("Lỗi khi xóa người dùng.");
					}
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn người dùng cần xóa!");
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (_currentUser.QuyenId > 2)
			{
				MessageBox.Show("Bạn không có quyền thêm người dùng!");
				return;
			}
			UserAddDialog dialog = new UserAddDialog(_currentUser.TenDangNhap);

			if (dialog.ShowDialog() == true)
			{
				LoadData(); // Reload danh sách sau khi thêm thành công
			}
		}
	}
}