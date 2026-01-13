using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class DiseaseManagementView : UserControl
	{
		// Khởi tạo các Controller
		private readonly DiseaseController _controller = new DiseaseController();
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public DiseaseManagementView()
		{
			InitializeComponent();

			// Load toàn bộ dữ liệu ban đầu
			LoadInitData();
			LoadOutbreaks();     // Load danh sách ổ dịch lên lưới
			LoadVaccinations();  // Load danh sách tiêm phòng lên lưới
		}

		// --- 1. KHỞI TẠO DỮ LIỆU CHUNG ---
		private void LoadInitData()
		{
			try
			{
				// 1. Load danh sách Bệnh (Dùng chung cho cả 2 tab)
				LoadDiseaseComboBoxes();

				// 2. Load danh sách Tỉnh (Chỉ dùng cho Tab 1: Ổ Dịch)
				// Lưu ý: Cần .DefaultView
				var dtTinh = _adminController.GetProvinces();
				if (dtTinh != null)
					cbTinhOD.ItemsSource = dtTinh.DefaultView;

				// 3. Load danh sách Ổ dịch (Dùng cho Tab 2: Tiêm Phòng)
				LoadOutbreakComboBox();

				// 4. Set ngày mặc định cho DatePicker
				dpNgayTiem.SelectedDate = DateTime.Now;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khởi tạo dữ liệu: " + ex.Message);
			}
		}

		// Hàm hỗ trợ load danh sách bệnh vào cả 2 ComboBox
		private void LoadDiseaseComboBoxes()
		{
			var dtBenh = _controller.GetDiseaseTypes();
			if (dtBenh != null)
			{
				cbBenhOD.ItemsSource = dtBenh.DefaultView; // Tab 1
				cbBenhTP.ItemsSource = dtBenh.DefaultView; // Tab 2
			}
		}

		// Hàm hỗ trợ load danh sách ổ dịch vào ComboBox bên tab Tiêm phòng
		private void LoadOutbreakComboBox()
		{
			var dtOdich = _controller.GetOutbreakList();
			if (dtOdich != null)
			{
				cbODichTP.ItemsSource = dtOdich.DefaultView;
			}
		}

		// --- 2. SỰ KIỆN THÊM LOẠI BỆNH MỚI (NÚT +) ---
		private void btnAddDiseaseType_Click(object sender, RoutedEventArgs e)
		{
			// Mở cửa sổ nhập liệu
			AddDiseaseTypeWindow addWindow = new AddDiseaseTypeWindow();

			// Nếu người dùng ấn Lưu
			if (addWindow.ShowDialog() == true)
			{
				string tenMoi = addWindow.TenBenh;
				string moTa = addWindow.MoTa;

				// Gọi Controller lưu vào DB
				bool result = _controller.AddDiseaseType(tenMoi, moTa);

				if (result)
				{
					MessageBox.Show("Thêm loại bệnh mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

					// Load lại ComboBox ở cả 2 tab để cập nhật tên bệnh vừa thêm
					LoadDiseaseComboBoxes();

					// Tự động chọn bệnh vừa thêm cho ComboBox ở tab hiện tại (Tab 1)
					cbBenhOD.Text = tenMoi;
				}
				else
				{
					MessageBox.Show("Thêm thất bại hoặc tên bệnh đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		// ============================================================
		// TAB 1: QUẢN LÝ Ổ DỊCH
		// ============================================================

		private void LoadOutbreaks()
		{
			dgOutbreak.ItemsSource = _controller.GetAllOutbreaks(txtSearchOD.Text).DefaultView;
		}

		private void txtSearchOD_TextChanged(object sender, TextChangedEventArgs e)
		{
			LoadOutbreaks();
		}

		// --- Xử lý địa chính (Tỉnh -> Huyện -> Xã) ---
		private void cbTinhOD_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbTinhOD.SelectedValue is int id)
			{
				cbHuyenOD.ItemsSource = _adminController.GetAllHuyen(id).DefaultView;
				cbXaOD.ItemsSource = null; // Reset xã
			}
		}

		private void cbHuyenOD_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbHuyenOD.SelectedValue is int id)
			{
				cbXaOD.ItemsSource = _adminController.GetAllXa(id).DefaultView;
			}
		}

		// --- Lưu Ổ Dịch ---
		private void btnSaveOutbreak_Click(object sender, RoutedEventArgs e)
		{
			// Validate dữ liệu
			if (cbXaOD.SelectedValue == null || cbBenhOD.SelectedValue == null || string.IsNullOrWhiteSpace(txtTenODich.Text))
			{
				MessageBox.Show("Vui lòng nhập tên ổ dịch, chọn bệnh và địa điểm (Xã)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				int soLuong = 0;
				int.TryParse(txtSoLuongOD.Text, out soLuong);

				// Gọi Controller thêm mới
				if (_controller.AddOutbreak(txtTenODich.Text,
											(int)cbBenhOD.SelectedValue,
											(int)cbXaOD.SelectedValue,
											soLuong,
											"", // Ghi chú (nếu có)
											txtChanDoanOD.Text,
											false))
				{
					MessageBox.Show("Thêm ổ dịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

					// 1. Refresh lưới hiển thị
					LoadOutbreaks();

					// 2. Refresh danh sách ổ dịch bên Tab Tiêm phòng (Để người dùng qua kia chọn được ngay)
					LoadOutbreakComboBox();

					// 3. Reset form
					txtTenODich.Clear();
					txtSoLuongOD.Clear();
					txtChanDoanOD.Clear();
					cbXaOD.SelectedIndex = -1;
					cbHuyenOD.SelectedIndex = -1;
					cbBenhOD.SelectedIndex = -1;
				}
				else
				{
					MessageBox.Show("Thêm thất bại. Vui lòng kiểm tra lại.", "Lỗi");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi: " + ex.Message);
			}
		}

		// --- Xóa Ổ Dịch ---
		private void btnDeleteOutbreak_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DataRowView row)
			{
				if (MessageBox.Show("Bạn có chắc chắn muốn xóa ổ dịch này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					if (_controller.DeleteOutbreak((int)row["Id"]))
					{
						LoadOutbreaks();
						// Refresh lại danh sách bên tab kia
						LoadOutbreakComboBox();
					}
					else
					{
						MessageBox.Show("Không thể xóa. Có thể ổ dịch này đã có dữ liệu tiêm phòng.", "Lỗi");
					}
				}
			}
		}


		// ============================================================
		// TAB 2: QUẢN LÝ TIÊM PHÒNG
		// ============================================================

		private void LoadVaccinations()
		{
			dgVaccine.ItemsSource = _controller.GetVaccinations(txtSearchTP.Text).DefaultView;
		}

		private void txtSearchTP_TextChanged(object sender, TextChangedEventArgs e)
		{
			LoadVaccinations();
		}

		// --- Lưu Đợt Tiêm Phòng ---
		private void btnSaveVaccine_Click(object sender, RoutedEventArgs e)
		{
			// Validate
			if (cbODichTP.SelectedValue == null || cbBenhTP.SelectedValue == null || string.IsNullOrEmpty(txtTenDotTP.Text))
			{
				MessageBox.Show("Vui lòng nhập Tên đợt, chọn Bệnh và Ổ Dịch cần tiêm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				int soLuong = 0;
				if (!int.TryParse(txtSoLuongTP.Text, out soLuong))
				{
					MessageBox.Show("Số lượng phải là số nguyên!", "Cảnh báo");
					return;
				}

				// SỬA LỖI: Đã xóa các ký tự lạ và format lại tham số
				if (_controller.AddVaccination(txtTenDotTP.Text,
											   (int)cbBenhTP.SelectedValue,
											   (int)cbODichTP.SelectedValue,
											   dpNgayTiem.SelectedDate ?? DateTime.Now,
											   txtLoaiVaccine.Text,
											   soLuong,
											   txtNguoiTH.Text))
				{
					MessageBox.Show("Thêm đợt tiêm phòng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					LoadVaccinations();

					// Reset form
					txtTenDotTP.Clear();
					txtSoLuongTP.Clear();
					txtLoaiVaccine.Clear();
					txtNguoiTH.Clear();
					cbODichTP.SelectedIndex = -1;
					cbBenhTP.SelectedIndex = -1;
				}
				else
				{
					MessageBox.Show("Thêm thất bại.", "Lỗi");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi: " + ex.Message);
			}
		}

		// --- Xóa Đợt Tiêm ---
		private void btnDeleteVaccine_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DataRowView row)
			{
				if (MessageBox.Show("Xóa đợt tiêm phòng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					if (_controller.DeleteVaccination((int)row["Id"]))
					{
						LoadVaccinations();
					}
				}
			}
		}
	}
}