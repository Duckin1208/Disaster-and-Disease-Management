using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class AdministrativeView : UserControl
	{
		// Khởi tạo Controller xử lý nghiệp vụ đơn vị hành chính
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public AdministrativeView()
		{
			InitializeComponent();
			LoadInitData();
		}

		// --- KHỞI TẠO DỮ LIỆU BAN ĐẦU ---
		private void LoadInitData()
		{
			try
			{
				// 1. Tải danh sách Tỉnh/Thành vào ComboBox
				DataTable dtTinh = _adminController.GetProvinces();
				if (cbTinh != null)
				{
					cbTinh.ItemsSource = dtTinh.DefaultView;
					cbTinh.DisplayMemberPath = "Ten";
					cbTinh.SelectedValuePath = "Id";

					if (dtTinh.Rows.Count > 0)
						cbTinh.SelectedIndex = 0; // Mặc định chọn tỉnh đầu tiên
				}

				// 2. Thiết lập mặc định cho ComboBox Cấp (Huyện/Xã)
				if (cbLevel != null)
				{
					cbLevel.SelectedIndex = 0; // Mặc định xem cấp Huyện
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khởi tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// --- XỬ LÝ TẢI DỮ LIỆU LÊN LƯỚI ---
		private void LoadData()
		{
			try
			{
				// Kiểm tra các điều khiển giao diện đã được khởi tạo chưa
				if (dgDonVi == null || cbTinh == null || cbTinh.SelectedValue == null || cbLevel == null) return;

				int tinhId = (int)cbTinh.SelectedValue;
				string keyword = txtSearch.Text.Trim();

				// Xác định ID cấp hành chính: Huyện (Index 0) = 2, Xã (Index 1) = 3
				int levelId = (cbLevel.SelectedIndex == 0) ? 2 : 3;

				// Gọi hàm Search từ Controller
				DataTable result = _adminController.Search(tinhId, levelId, keyword);
				dgDonVi.ItemsSource = result.DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// --- CÁC SỰ KIỆN THAY ĐỔI LỰA CHỌN ---
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadData();
		}

		private void cbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadData();
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			LoadData();
		}

		// --- CHỨC NĂNG XÓA ĐƠN VỊ ---
		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (dgDonVi.SelectedItem is DataRowView row)
			{
				string ten = row["Ten"].ToString();
				int id = (int)row["Id"];

				var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn vị: {ten}?",
					"Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (confirm == MessageBoxResult.Yes)
				{
					if (_adminController.Delete(id))
					{
						MessageBox.Show("Xóa thành công!", "Thông báo");
						LoadData();
					}
					else
					{
						MessageBox.Show("Không thể xóa. Đơn vị này có thể đang chứa dữ liệu con hoặc đang được sử dụng ở bảng khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Stop);
					}
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn một dòng trên bảng để xóa.", "Thông báo");
			}
		}
		// 1. CHỨC NĂNG THÊM HUYỆN 
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (cbTinh.SelectedValue == null)
			{
				MessageBox.Show("Vui lòng chọn một Tỉnh/Thành phố trước!", "Thông báo");
				return;
			}

			int provinceId = (int)cbTinh.SelectedValue;
			string provinceName = cbTinh.Text;

			// Sử dụng InputBox để hỏi tên Huyện mới
			string newDistrictName = Microsoft.VisualBasic.Interaction.InputBox(
				$"Nhập tên Quận/Huyện mới thuộc {provinceName}:", "Thêm Huyện Mới", "");

			if (!string.IsNullOrWhiteSpace(newDistrictName))
			{
				// Cấp 2 = Huyện. Controller sẽ tự kế thừa Chi cục từ Tỉnh (provinceId)
				if (_adminController.Add(newDistrictName, 2, provinceId))
				{
					MessageBox.Show($"Đã thêm huyện '{newDistrictName}' thành công. Chi cục đã được gán tự động.", "Thành công");
					cbLevel.SelectedIndex = 0; // Chuyển về xem Huyện
					LoadData();
				}
				else
				{
					MessageBox.Show("Lỗi khi thêm Huyện mới.");
				}
			}
		}

		// 2. CHỨC NĂNG THÊM NHANH XÃ 
		private void btnQuickAdd_Click(object sender, RoutedEventArgs e)
		{
			string newCommuneName = txtQuickAddName.Text.Trim();
			if (string.IsNullOrEmpty(newCommuneName))
			{
				MessageBox.Show("Vui lòng nhập tên xã mới!", "Cảnh báo");
				return;
			}

			if (dgDonVi.SelectedItem is DataRowView selectedRow)
			{
				int parentId = (int)selectedRow["Id"];
				string capHienTai = selectedRow["Cap"].ToString();

				// Sử dụng Contains để tránh lỗi do chuỗi "Quận/Huyện"
				if (!capHienTai.Contains("Huyện"))
				{
					MessageBox.Show("Để thêm xã, bạn phải chọn một đơn vị cấp HUYỆN trên bảng dữ liệu.", "Lỗi");
					return;
				}

				// Cấp 3 = Xã. Controller sẽ tự kế thừa Chi cục từ Huyện (parentId)
				if (_adminController.Add(newCommuneName, 3, parentId))
				{
					MessageBox.Show($"Đã thêm xã '{newCommuneName}' thành công!", "Thành công");
					txtQuickAddName.Clear();
					cbLevel.SelectedIndex = 1; // Chuyển sang xem Xã để thấy kết quả
					LoadData();
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn một Huyện trên bảng trước khi thêm xã con.", "Thông báo");
			}
		}
	}
}