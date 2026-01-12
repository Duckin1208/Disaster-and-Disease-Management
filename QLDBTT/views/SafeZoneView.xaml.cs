using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class SafeZoneView : UserControl
	{
		private readonly FacilityManagementController _controller = new FacilityManagementController();

		public SafeZoneView()
		{
			InitializeComponent();
			LoadData();
		}

		private void LoadData()
		{
			try
			{
				string keyword = txtSearch.Text.Trim();
				dgSafeZones.ItemsSource = _controller.SearchSafeZones(keyword).DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
			}
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadData();
		}

		private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			LoadData();
		}

		private void btnDetail_Click(object sender, RoutedEventArgs e)
		{
			if (dgSafeZones.SelectedItem is DataRowView row)
			{
				string ten = row["Ten"].ToString();
				string giayPhep = row["SoGiayPhep"].ToString();
				string trangThai = row["TrangThai"].ToString();

				MessageBox.Show($"THÔNG TIN CHI TIẾT:\n\n" +
								$"- Cơ sở/Vùng: {ten}\n" +
								$"- Số giấy phép: {giayPhep}\n" +
								$"- Trạng thái: {trangThai}\n\n" +
								$"(Chức năng xem bản scan giấy phép đang cập nhật)",
								"Chi tiết", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void btnExport_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Đã xuất danh sách ra file Excel thành công! (Giả lập)", "Thông báo");
		}
	}
}