using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // OpenFileDialog
using System.Diagnostics; // Process.Start
using System.Data; // [QUAN TRỌNG] Để dùng .DefaultView
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Views
{
	public partial class DisasterManagementView : UserControl
	{
		private readonly DisasterController _controller = new DisasterController();
		private readonly AdministrativeController _adminCtrl = new AdministrativeController();
		private string _currentType = "Trượt lở";

		public DisasterManagementView()
		{
			InitializeComponent();
			LoadInitData();
			LoadData();
		}

		private void LoadInitData()
		{
			try
			{
				// 1. Nạp danh sách TỈNH (Cấp 1) vào cbTinh
				var dtTinh = _adminCtrl.GetProvinces();

				cbTinh.ItemsSource = dtTinh.DefaultView;
				cbTinh.DisplayMemberPath = "Ten";
				cbTinh.SelectedValuePath = "Id";

				// Mặc định chọn dòng đầu (VD: Hà Nội)
				if (dtTinh.Rows.Count > 0)
					cbTinh.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khởi tạo danh mục: " + ex.Message);
			}
		}

		// Khi chọn Tỉnh -> Nạp danh sách Huyện tương ứng
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cbTinh.SelectedValue != null)
				{
					int tinhId = (int)cbTinh.SelectedValue;

					// Nạp các huyện thuộc tỉnh đó vào cbDonVi
					cbDonVi.ItemsSource = _adminCtrl.GetAllHuyen(tinhId).DefaultView;
					cbDonVi.DisplayMemberPath = "Ten";
					cbDonVi.SelectedValuePath = "Id";
					cbDonVi.SelectedIndex = -1; // Reset lựa chọn
				}
			}
			catch { }
		}

		private void LoadData()
		{
			try
			{
				if (dgThienTai == null) return;
				dgThienTai.ItemsSource = _controller.GetListByType(_currentType);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
			}
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (rbTruotLo == null || rbLuQuet == null) return;

			if (rbTruotLo.IsChecked == true)
				_currentType = "Trượt lở";
			else
				_currentType = "Lũ quét";

			LoadData();
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			string keyword = txtSearch.Text.Trim();
			dgThienTai.ItemsSource = _controller.Search(_currentType, keyword);
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra: Phải chọn Tỉnh và Huyện
			if (cbDonVi.SelectedValue == null)
			{
				MessageBox.Show("Vui lòng chọn Địa điểm (Tỉnh -> Huyện)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				cbDonVi.Focus();
				return;
			}

			if (cbMucDo.SelectedItem == null)
			{
				MessageBox.Show("Vui lòng chọn Mức độ thiên tai!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				cbMucDo.Focus();
				return;
			}

			try
			{
				// Lấy ID Huyện
				int donViId = (int)cbDonVi.SelectedValue;

				string mucDoStr = (cbMucDo.SelectedItem as ComboBoxItem)?.Content.ToString();
				int mucDo = int.Parse(mucDoStr ?? "1");
				string filePath = txtFilePath.Text;

				// Thêm vào CSDL
				int newId = _controller.Add(donViId, _currentType, mucDo);

				if (newId > 0)
				{
					// Xử lý file đính kèm
					if (!string.IsNullOrEmpty(filePath))
					{
						_controller.UploadReportFile(newId, filePath);
					}

					MessageBox.Show("Thêm báo cáo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					LoadData();
					ClearInputs();
				}
				else
				{
					MessageBox.Show("Lỗi: Không thể thêm vào cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi hệ thống khi thêm mới: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (dgThienTai.SelectedItem is DiemThienTai selected)
			{
				var confirm = MessageBox.Show($"Bạn có chắc muốn xóa báo cáo tại {selected.TenDonVi}?",
											  "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

				if (confirm == MessageBoxResult.Yes)
				{
					if (_controller.Delete(selected.Id))
					{
						MessageBox.Show("Đã xóa báo cáo và file đính kèm.");
						LoadData();
					}
					else
					{
						MessageBox.Show("Không thể xóa báo cáo này.");
					}
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn một dòng để xóa.");
			}
		}

		private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Tài liệu báo cáo|*.pdf;*.doc;*.docx;*.jpg;*.png";

			if (openFileDialog.ShowDialog() == true)
			{
				txtFilePath.Text = openFileDialog.FileName;
			}
		}

		private void btnViewFile_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DiemThienTai item)
			{
				string path = FileAttachmentHelper.GetFilePath(item.Id);
				if (path != null)
				{
					try
					{
						Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
					}
					catch (Exception ex)
					{
						MessageBox.Show("Không thể mở file: " + ex.Message);
					}
				}
				else
				{
					MessageBox.Show("Báo cáo này không có file đính kèm.");
				}
			}
		}

		private void ClearInputs()
		{
			txtFilePath.Text = "";
			cbDonVi.SelectedIndex = -1;
			// Không reset cbTinh để tiện nhập tiếp
			cbMucDo.SelectedIndex = 0;
		}
	}
}