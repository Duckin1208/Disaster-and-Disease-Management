using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; 
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class CertificateWindow : Window
	{
		private readonly CertificateController _controller = new CertificateController();
		private int _coSoId;
		private int _selectedLicenseId = -1;

		public CertificateWindow(int coSoId, string tenCoSo)
		{
			InitializeComponent();
			_coSoId = coSoId;

			if (lblSubTitle != null)
				lblSubTitle.Text = $"Cơ sở: {tenCoSo}";

			// Thiết lập ngày mặc định
			dpNgayCap.SelectedDate = DateTime.Now;
			dpNgayHetHan.SelectedDate = DateTime.Now.AddYears(1);

			LoadCertificates();
			ToggleConditionForm(false);
		}

		private void LoadCertificates()
		{
			try
			{
				dgCertificates.ItemsSource = _controller.GetCertificates(_coSoId).DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải danh sách giấy phép: " + ex.Message);
			}
		}

		private void btnAddCert_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtSoGiayPhep.Text) || dpNgayCap.SelectedDate == null || dpNgayHetHan.SelectedDate == null)
			{
				MessageBox.Show("Vui lòng điền đầy đủ thông tin giấy phép!");
				return;
			}

			// CHỨC NĂNG MỚI: Kiểm tra tính hợp lệ của ngày tháng
			if (dpNgayHetHan.SelectedDate <= dpNgayCap.SelectedDate)
			{
				MessageBox.Show("Lỗi: Ngày hết hạn phải sau ngày cấp giấy phép!", "Dữ liệu sai", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (_controller.AddCertificate(txtSoGiayPhep.Text, dpNgayCap.SelectedDate.Value, dpNgayHetHan.SelectedDate.Value, _coSoId))
			{
				MessageBox.Show("Đã cấp giấy phép mới thành công!");
				LoadCertificates();
				txtSoGiayPhep.Clear();
			}
		}

		private void btnDeleteCert_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DataRowView row)
			{
				int id = (int)row["Id"];
				if (MessageBox.Show("Xác nhận xóa giấy phép và toàn bộ điều kiện đính kèm?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					_controller.DeleteCertificate(id);
					LoadCertificates();
					dgConditions.ItemsSource = null;
					ToggleConditionForm(false);
				}
			}
		}

		private void dgCertificates_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dgCertificates.SelectedItem is DataRowView row)
			{
				_selectedLicenseId = (int)row["Id"];
				ToggleConditionForm(true);
				LoadConditions();

				// Cập nhật thanh trạng thái
				int count = dgConditions.Items.Count;
				txtStatusInfo.Text = $"Đang chọn GP: {row["SoGiayPhep"]} | Số điều kiện: {count}";
			}
			else
			{
				_selectedLicenseId = -1;
				ToggleConditionForm(false);
				dgConditions.ItemsSource = null;
				txtStatusInfo.Text = "Vui lòng chọn giấy phép để xem chi tiết";
			}
		}

		private void LoadConditions()
		{
			if (_selectedLicenseId != -1)
			{
				dgConditions.ItemsSource = _controller.GetConditions(_selectedLicenseId).DefaultView;
			}
		}

		private void btnAddCond_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedLicenseId == -1) return;

			if (string.IsNullOrWhiteSpace(txtCondName.Text))
			{
				MessageBox.Show("Vui lòng nhập tên điều kiện/tiêu chí!");
				return;
			}

			if (_controller.AddCondition(txtCondName.Text, txtCondDesc.Text, _selectedLicenseId))
			{
				LoadConditions();
				txtCondName.Clear();
				txtCondDesc.Clear();

				// Cập nhật lại thanh trạng thái sau khi thêm mới
				if (dgCertificates.SelectedItem is DataRowView row)
					txtStatusInfo.Text = $"Đang chọn GP: {row["SoGiayPhep"]} | Số điều kiện: {dgConditions.Items.Count}";
			}
		}

		private void btnDeleteCond_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DataRowView row)
			{
				int id = (int)row["Id"];
				if (_controller.DeleteCondition(id))
				{
					LoadConditions();
					// Cập nhật lại thanh trạng thái sau khi xóa
					if (dgCertificates.SelectedItem is DataRowView certRow)
						txtStatusInfo.Text = $"Đang chọn GP: {certRow["SoGiayPhep"]} | Số điều kiện: {dgConditions.Items.Count}";
				}
			}
		}

		private void ToggleConditionForm(bool isEnabled)
		{
			btnAddCond.IsEnabled = isEnabled;
			txtCondName.IsEnabled = isEnabled;
			txtCondDesc.IsEnabled = isEnabled;
			if (!isEnabled) { txtCondName.Clear(); txtCondDesc.Clear(); }
		}
	}
}