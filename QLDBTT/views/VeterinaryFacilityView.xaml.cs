using System.Windows;
using System.Windows.Controls;
using System.Data;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Views
{
	public partial class VeterinaryFacilityView : UserControl
	{
		private readonly FacilityManagementController _controller = new FacilityManagementController();
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public VeterinaryFacilityView()
		{
			InitializeComponent();
			LoadInitData();
		}

		private void LoadInitData()
		{
			try
			{
				var types = _controller.GetFacilityTypes();

				// Nạp ComboBox Lọc
				cbFilterType.ItemsSource = types;
				if (types.Count > 0) cbFilterType.SelectedIndex = 0;

				// Nạp ComboBox Thêm mới - Loại hình
				cbLoaiAdd.ItemsSource = types;

				// Nạp danh sách TỈNH (Cấp 1) trước
				// GetProvinces() để lấy 63 tỉnh
				var dtTinh = _adminController.GetProvinces();

				cbTinh.ItemsSource = dtTinh.DefaultView;
				// Mặc định chọn dòng đầu tiên (Ví dụ: Hà Nội)
				if (dtTinh.Rows.Count > 0)
					cbTinh.SelectedIndex = 0;
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Lỗi khởi tạo: " + ex.Message);
			}
		}

		private void LoadData()
		{
			if (cbFilterType.SelectedValue == null) return;

			string typeName = "";
			if (cbFilterType.SelectedValue is string val)
				typeName = val;
			else if (cbFilterType.SelectedItem is LoaiCoSo item)
				typeName = item.Ten;
			else
				typeName = cbFilterType.SelectedValue.ToString();

			string keyword = txtSearch.Text.Trim();
			dgFacilities.ItemsSource = _controller.GetFacilitiesByType(typeName, keyword);
		}

		// Chọn Tỉnh để lọc Huyện
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cbTinh.SelectedValue != null)
				{
					int tinhId = (int)cbTinh.SelectedValue;

					// Gọi hàm lấy Huyện theo ID Tỉnh vừa chọn
					cbDonVi.ItemsSource = _adminController.GetAllHuyen(tinhId).DefaultView;
					cbDonVi.DisplayMemberPath = "Ten";
					cbDonVi.SelectedValuePath = "Id";

					// Reset chọn huyện
					cbDonVi.SelectedIndex = -1;
				}
			}
			catch { }
		}

		private void cbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadData();
		private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra phải chọn cả Loại hình và Huyện (Địa điểm cụ thể)
			if (cbLoaiAdd.SelectedValue == null || cbDonVi.SelectedValue == null)
			{
				MessageBox.Show("Vui lòng chọn Loại hình và Địa điểm (Tỉnh -> Huyện)!");
				return;
			}

			int loaiId = (int)cbLoaiAdd.SelectedValue;
			int donViId = (int)cbDonVi.SelectedValue; // Đây là ID Huyện

			if (_controller.AddFacility(txtTenCoSo.Text, loaiId, donViId, txtQuyMo.Text, txtSDT.Text))
			{
				MessageBox.Show("Thêm thành công!");
				LoadData();

				// Clear form
				txtTenCoSo.Clear();
				txtQuyMo.Clear();
				txtSDT.Clear();
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			var item = ((Button)sender).DataContext as CoSoHienThi;
			if (item != null)
			{
				if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					if (_controller.DeleteFacility(item.Id)) LoadData();
				}
			}
		}

		private void btnLicense_Click(object sender, RoutedEventArgs e)
		{
			var item = ((Button)sender).DataContext as CoSoHienThi;
			if (item != null)
			{
				CertificateWindow win = new CertificateWindow(item.Id, item.Ten);
				win.ShowDialog();
			}
		}

		private void btnStats_Click(object sender, RoutedEventArgs e)
		{
			HouseholdStatsWindow win = new HouseholdStatsWindow();
			win.ShowDialog();
		}
	}
}