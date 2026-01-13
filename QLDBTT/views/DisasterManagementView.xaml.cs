using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Views
{
	public partial class DisasterManagementView : UserControl
	{
		// Khởi tạo Controller
		// Lưu ý: Cần chắc chắn bạn đã có AdministrativeController để lấy danh sách địa chính
		private readonly DisasterController _disasterController = new DisasterController();
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public DisasterManagementView()
		{
			InitializeComponent();
			this.Loaded += DisasterManagementView_Loaded;
		}

		private void DisasterManagementView_Loaded(object sender, RoutedEventArgs e)
		{
			LoadInitData();
		}

		// --- 1. KHỞI TẠO DỮ LIỆU ---
		private void LoadInitData()
		{
			try
			{
				// Load danh sách thiên tai lên lưới
				LoadDataGrid();

				// Load ComboBox Loại Thiên Tai
				var dtLoai = _disasterController.GetDisasterTypes();
				if (dtLoai != null)
				{
					cbLoai.ItemsSource = dtLoai.DefaultView;
				}

				// Load ComboBox TỈNH (Cấp cao nhất)
				LoadProvinces();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khởi tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadDataGrid()
		{
			string keyword = txtSearch.Text.Trim();
			dgDisasters.ItemsSource = _disasterController.GetAllDisasters(keyword);
		}

		// --- 2. LOGIC CASCADING DROPDOWN (TỈNH -> HUYỆN -> XÃ) ---

		// Load danh sách Tỉnh
		private void LoadProvinces()
		{
			var dtTinh = _adminController.GetProvinces();
			if (dtTinh != null)
			{
				cbTinh.ItemsSource = dtTinh.DefaultView;
			}
		}

		// Khi chọn Tỉnh -> Tải Huyện
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Reset Huyện và Xã
			cbHuyen.ItemsSource = null;
			cbHuyen.IsEnabled = false;
			cbXa.ItemsSource = null;
			cbXa.IsEnabled = false;

			if (cbTinh.SelectedValue != null && int.TryParse(cbTinh.SelectedValue.ToString(), out int provinceId))
			{
				var dtHuyen = _adminController.GetAllHuyen(provinceId);
				if (dtHuyen != null && dtHuyen.Rows.Count > 0)
				{
					cbHuyen.ItemsSource = dtHuyen.DefaultView;
					cbHuyen.IsEnabled = true;
				}
			}
		}

		// Khi chọn Huyện -> Tải Xã
		private void cbHuyen_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Reset Xã
			cbXa.ItemsSource = null;
			cbXa.IsEnabled = false;

			if (cbHuyen.SelectedValue != null && int.TryParse(cbHuyen.SelectedValue.ToString(), out int districtId))
			{
				var dtXa = _adminController.GetAllXa(districtId);
				if (dtXa != null && dtXa.Rows.Count > 0)
				{
					cbXa.ItemsSource = dtXa.DefaultView;
					cbXa.IsEnabled = true;
				}
			}
		}

		// --- 3. CÁC CHỨC NĂNG CHÍNH (TÌM, LƯU, XÓA) ---

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadDataGrid();
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// 1. Validate dữ liệu
				if (cbLoai.SelectedValue == null)
				{
					MessageBox.Show("Vui lòng chọn Loại thiên tai!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
					cbLoai.Focus();
					return;
				}

				// 2. Xác định Đơn vị ID cuối cùng được chọn
				// Ưu tiên Xã -> Huyện -> Tỉnh
				int finalDonViId = 0;
				if (cbXa.SelectedValue != null)
				{
					finalDonViId = (int)cbXa.SelectedValue;
				}
				else if (cbHuyen.SelectedValue != null)
				{
					finalDonViId = (int)cbHuyen.SelectedValue;
				}
				else if (cbTinh.SelectedValue != null)
				{
					finalDonViId = (int)cbTinh.SelectedValue;
				}
				else
				{
					MessageBox.Show("Vui lòng chọn địa điểm (Tỉnh/Huyện/Xã)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// 3. Lấy dữ liệu khác
				int loaiId = (int)cbLoai.SelectedValue;
				string ghiChu = txtGhiChu.Text;
				string filePath = txtFilePath.Text;

				int mucDo = 1;
				if (cbMucDo.SelectedItem is ComboBoxItem item && item.Tag != null)
				{
					int.TryParse(item.Tag.ToString(), out mucDo);
				}

				// 4. Gọi Controller lưu
				if (_disasterController.AddDisaster(finalDonViId, loaiId, mucDo, ghiChu, filePath))
				{
					MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					ResetForm();
					LoadDataGrid();
				}
				else
				{
					MessageBox.Show("Thêm thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DiemThienTai item)
			{
				var result = MessageBox.Show($"Bạn có chắc chắn xóa bản ghi tại '{item.TenDonVi}'?",
											 "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

				if (result == MessageBoxResult.Yes)
				{
					if (_disasterController.DeleteDisaster(item.Id))
					{
						MessageBox.Show("Đã xóa thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
						LoadDataGrid();
					}
					else
					{
						MessageBox.Show("Không thể xóa bản ghi này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		// --- 4. XỬ LÝ FILE ---

		private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				Filter = "Tài liệu (*.pdf;*.doc;*.docx;*.xls)|*.pdf;*.doc;*.docx;*.xls|Ảnh (*.png;*.jpg)|*.png;*.jpg|Tất cả|*.*",
				Title = "Chọn file báo cáo"
			};

			if (dlg.ShowDialog() == true)
			{
				txtFilePath.Text = dlg.FileName;
			}
		}

		private void btnViewFile_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DiemThienTai item)
			{
				if (!string.IsNullOrEmpty(item.FileDinhKem))
				{
					try
					{
						_disasterController.OpenAttachment(item.Id);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Không thể mở file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
				else
				{
					MessageBox.Show("Không có file đính kèm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}

		// --- 5. HELPER ---

		private void ResetForm()
		{
			// Reset Dropdowns
			cbTinh.SelectedIndex = -1;
			// Huyện và Xã sẽ tự động reset nhờ sự kiện SelectionChanged của Tỉnh

			cbLoai.SelectedIndex = -1;
			cbMucDo.SelectedIndex = 0;
			txtGhiChu.Clear();
			txtFilePath.Clear();
		}
	}
}