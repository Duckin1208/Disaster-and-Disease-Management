using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // Thư viện cho OpenFileDialog
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;
using Disease_Disaster.Helpers;
using System.Data; // Cần thiết để nhận diện thuộc tính DefaultView

namespace Disease_Disaster.Views
{
	public partial class DisasterManagementView : UserControl
	{
		// Khởi tạo Controller và Helper
		private readonly DisasterController _controller = new DisasterController();
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		public DisasterManagementView()
		{
			InitializeComponent();
			LoadInitData();
		}

		// --- 1. KHỞI TẠO DỮ LIỆU BAN ĐẦU ---
		private void LoadInitData()
		{
			try
			{
				// Load danh sách dữ liệu lên lưới
				LoadDataGrid();

				// Load ComboBox Loại Thiên Tai
				cbLoai.ItemsSource = _controller.GetDisasterTypes().DefaultView;

				// Load ComboBox Đơn Vị (SỬA LỖI: Dùng bảng DonVi thay vì DonViHanhChinh)
				// Lưu ý: Phải có .DefaultView để Binding hoạt động trơn tru
				cbDonVi.ItemsSource = _dbHelper.ExecuteQuery("SELECT Id, Ten FROM DonVi ORDER BY Ten").DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khởi tạo dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// --- 2. HÀM LOAD DỮ LIỆU LÊN DATAGRID ---
		private void LoadDataGrid()
		{
			try
			{
				// Lấy từ khóa tìm kiếm từ TextBox (txtSearch)
				string keyword = txtSearch.Text;

				// Gọi Controller lấy danh sách và gán vào DataGrid
				dgDisasters.ItemsSource = _controller.GetAllDisasters(keyword);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải danh sách: " + ex.Message);
			}
		}

		// --- 3. SỰ KIỆN TÌM KIẾM ---
		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadDataGrid();
		}

		// --- 4. SỰ KIỆN XÓA ---
		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DiemThienTai item)
			{
				if (MessageBox.Show($"Bạn có chắc chắn muốn xóa bản ghi tại '{item.TenDonVi}' không?",
									"Xác nhận xóa",
									MessageBoxButton.YesNo,
									MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					if (_controller.DeleteDisaster(item.Id))
					{
						MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
						LoadDataGrid(); // Load lại lưới
					}
					else
					{
						MessageBox.Show("Xóa thất bại. Có thể dữ liệu đang bị ràng buộc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		// --- 5. SỰ KIỆN XEM FILE ĐÍNH KÈM ---
		private void btnViewFile_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DiemThienTai item)
			{
				if (item.TrangThaiFile == "Có đính kèm")
				{
					_controller.OpenAttachment(item.Id);
				}
				else
				{
					MessageBox.Show("Bản ghi này không có file đính kèm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}

		// --- 6. SỰ KIỆN CHỌN FILE TỪ MÁY TÍNH (BROWSE) ---
		private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Tài liệu|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.jpg;*.png|Tất cả các file|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				txtFilePath.Text = openFileDialog.FileName;
			}
		}

		// --- 7. SỰ KIỆN THÊM MỚI (LƯU) ---
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Validate dữ liệu
				if (cbDonVi.SelectedValue == null)
				{
					MessageBox.Show("Vui lòng chọn Đơn vị hành chính!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
					cbDonVi.Focus();
					return;
				}
				if (cbLoai.SelectedValue == null)
				{
					MessageBox.Show("Vui lòng chọn Loại thiên tai!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
					cbLoai.Focus();
					return;
				}

				// Lấy giá trị
				int donViId = Convert.ToInt32(cbDonVi.SelectedValue);
				int loaiId = Convert.ToInt32(cbLoai.SelectedValue);
				string ghiChu = txtGhiChu.Text;
				string filePath = txtFilePath.Text;

				// Lấy mức độ
				int mucDo = 1;
				if (cbMucDo.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
				{
					int.TryParse(selectedItem.Tag.ToString(), out mucDo);
				}

				// Gọi Controller
				bool isSuccess = _controller.AddDisaster(donViId, loaiId, mucDo, ghiChu, filePath);

				if (isSuccess)
				{
					MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					LoadDataGrid();

					// Reset form
					txtGhiChu.Clear();
					txtFilePath.Clear();
					cbDonVi.SelectedIndex = -1;
					cbLoai.SelectedIndex = -1;
					cbMucDo.SelectedIndex = 0;
				}
				else
				{
					MessageBox.Show("Thêm thất bại. Vui lòng kiểm tra lại kết nối hoặc dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}