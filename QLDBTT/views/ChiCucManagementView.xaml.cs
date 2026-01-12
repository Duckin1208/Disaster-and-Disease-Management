using System.Data;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class ChiCucManagementView : UserControl
	{
		private readonly ChiCucController _controller = new ChiCucController();

		public ChiCucManagementView()
		{
			InitializeComponent();
			LoadInitData();
		}

		private void LoadInitData()
		{
			// Nạp danh sách Chi cục
			dgChiCuc.ItemsSource = _controller.GetAllChiCuc().DefaultView;
			// Nạp danh sách Tỉnh chưa phân công
			LoadAvailableProvinces();
		}

		private void LoadAvailableProvinces()
		{
			cbAvailableProv.ItemsSource = _controller.GetAvailableProvinces().DefaultView;
		}

		// Khi chọn 1 dòng Chi cục -> Load danh sách tỉnh tương ứng
		private void dgChiCuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dgChiCuc.SelectedItem is DataRowView row)
			{
				int chiCucId = (int)row["Id"];
				lblSelectedChiCuc.Text = row["Ten"].ToString();

				// Lấy các tỉnh thuộc chi cục này
				dgProvinces.ItemsSource = _controller.GetProvincesByChiCuc(chiCucId).DefaultView;
			}
		}

		// Thêm Tỉnh vào Chi cục đang chọn
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (dgChiCuc.SelectedItem is DataRowView row && cbAvailableProv.SelectedValue != null)
			{
				int chiCucId = (int)row["Id"];
				int donViId = (int)cbAvailableProv.SelectedValue;

				if (_controller.UpdateProvinceChiCuc(donViId, chiCucId))
				{
					MessageBox.Show("Đã thêm địa bàn thành công!");
					// Refresh dữ liệu
					dgChiCuc_SelectionChanged(null, null); // Load lại bảng phải
					LoadAvailableProvinces(); // Load lại combobox
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn một Chi cục (bên trái) và một Tỉnh (bên dưới) để thêm.");
			}
		}

		// Gỡ bỏ Tỉnh khỏi Chi cục
		private void btnRemove_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			if (btn.DataContext is DataRowView row)
			{
				int donViId = (int)row["Id"];
				string tenTinh = row["Ten"].ToString();

				if (MessageBox.Show($"Bạn có chắc chắn muốn gỡ '{tenTinh}' khỏi chi cục này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					// Truyền null vào tham số thứ 2 để gỡ bỏ
					if (_controller.UpdateProvinceChiCuc(donViId, null))
					{
						dgChiCuc_SelectionChanged(null, null); // Refresh bảng
						LoadAvailableProvinces(); // Refresh combobox
					}
				}
			}
		}
	}
}