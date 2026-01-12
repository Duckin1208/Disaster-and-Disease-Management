using System;
using System.Data; 
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;
using Microsoft.VisualBasic; 
namespace Disease_Disaster.Views
{
	public partial class AdministrativeView : UserControl
	{
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public AdministrativeView()
		{
			InitializeComponent();
			LoadInitData();
		}

		private void LoadInitData()
		{
			try
			{
				// Lấy danh sách TẤT CẢ Tỉnh/Thành phố (Cấp 1)
				// Hàm GetProvinces() đã được định nghĩa trong Controller
				DataTable dtTinh = _adminController.GetProvinces();

				if (cbTinh != null)
				{
					// Gán dữ liệu vào ComboBox
					cbTinh.ItemsSource = dtTinh.DefaultView;
					cbTinh.DisplayMemberPath = "Ten";
					cbTinh.SelectedValuePath = "Id";

					// Mặc định chọn dòng đầu tiên (thường là Hà Nội hoặc tỉnh đầu bảng chữ cái)
					if (dtTinh.Rows.Count > 0)
						cbTinh.SelectedIndex = 0;
				}

				// Setup ComboBox Cấp Hành Chính (Huyện/Xã)
				if (cbLevel != null && cbLevel.Items.Count == 0)
				{
					cbLevel.Items.Add("Quận/Huyện/Thị xã"); // Index 0
					cbLevel.Items.Add("Phường/Xã/Thị trấn"); // Index 1
					cbLevel.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khởi tạo: {ex.Message}");
			}
		}

		// Hàm tải dữ liệu lên lưới (DataGrid)
		private void LoadData()
		{
			try
			{
				if (dgDonVi == null || cbTinh == null || cbTinh.SelectedValue == null) return;

				// 1. Lấy ID của Tỉnh đang được chọn trên ComboBox 
				int tinhId = (int)cbTinh.SelectedValue;

				string keyword = txtSearch.Text.Trim();

				// 2. Xác định cấp muốn xem (2 = Huyện, 3 = Xã)
				// Index 0 là Huyện, Index 1 là Xã
				int levelId = (cbLevel.SelectedIndex == 0) ? 2 : 3;

				// 3. Gọi hàm Search trong Controller
				// Hàm này sẽ tìm các đơn vị thuộc Tỉnh (tinhId) có Cấp tương ứng (levelId)
				DataTable result = _adminController.Search(tinhId, levelId, keyword);

				dgDonVi.ItemsSource = result.DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
			}
		}

		// Khi chọn tỉnh khác -> Tự động tải lại dữ liệu của tỉnh đó
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Reset về xem cấp Huyện để tránh nhầm lẫn khi đổi tỉnh
			if (cbLevel != null) cbLevel.SelectedIndex = 0;
			LoadData();
		}

		// Khi đổi cấp (Huyện <-> Xã) -> Tải lại dữ liệu
		private void cbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadData();
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadData();
		}

		// Xóa đơn vị
		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (dgDonVi.SelectedItem is DataRowView row)
			{
				string ten = row["Ten"].ToString();
				int id = (int)row["Id"];

				var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa: {ten}?",
											 "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					if (_adminController.Delete(id))
					{
						MessageBox.Show("Xóa thành công!");
						LoadData();
					}
					else
					{
						MessageBox.Show("Không thể xóa. Đơn vị này có thể đang chứa đơn vị con (Xã/Phường) hoặc dữ liệu liên quan.");
					}
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn một dòng để xóa.");
			}
		}

		// Thêm đơn vị mới
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (cbTinh.SelectedValue == null) return;

			int parentId;
			int levelToAdd;

			// -- Logic xác định Cha/Con --

			// Trường hợp 1: Đang xem Huyện -> Muốn thêm Huyện mới -> Cha là Tỉnh đang chọn
			if (cbLevel.SelectedIndex == 0)
			{
				parentId = (int)cbTinh.SelectedValue; // Lấy ID Tỉnh từ ComboBox
				levelToAdd = 2; // Cấp Huyện
			}
			// Trường hợp 2: Đang xem Xã -> Muốn thêm Xã mới -> Cha là Huyện
			else
			{
				// Yêu cầu người dùng phải đang chọn một Huyện trên lưới (hoặc chuyển về view Huyện)
				// Để đơn giản: Bắt buộc về màn hình Huyện, chọn Huyện rồi bấm Thêm
				MessageBox.Show("Để thêm Xã/Phường: Vui lòng chuyển ComboBox 'Cấp' về 'Quận/Huyện', chọn Huyện cha trong danh sách, sau đó bấm nút Thêm.", "Hướng dẫn");
				return;
			}

			// Hiển thị hộp thoại nhập tên
			string newName = Interaction.InputBox("Nhập tên đơn vị hành chính mới:", "Thêm Mới", "");

			if (!string.IsNullOrWhiteSpace(newName))
			{
				// Nếu đang ở màn hình Huyện mà chọn 1 dòng -> Hệ thống hiểu là muốn thêm Xã vào Huyện đó
				if (cbLevel.SelectedIndex == 0 && dgDonVi.SelectedItem is DataRowView selectedHuyen)
				{
					// User đang chọn 1 Huyện, và bấm thêm. Ta hiểu là thêm Xã vào Huyện này.
					if (MessageBox.Show($"Bạn muốn thêm '{newName}' làm Xã/Phường trực thuộc '{selectedHuyen["Ten"]}' phải không?\n\n(Chọn No nếu bạn muốn thêm Huyện mới trực thuộc Tỉnh)", "Xác nhận cấp cha", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					{
						parentId = (int)selectedHuyen["Id"];
						levelToAdd = 3;
					}
				}

				if (_adminController.Add(newName, levelToAdd, parentId))
				{
					MessageBox.Show("Thêm thành công!");

					// Nếu vừa thêm Xã, tự động chuyển view sang Xã
					if (levelToAdd == 3) cbLevel.SelectedIndex = 1;

					LoadData();
				}
				else
				{
					MessageBox.Show("Lỗi khi thêm đơn vị.");
				}
			}
		}
	}
}