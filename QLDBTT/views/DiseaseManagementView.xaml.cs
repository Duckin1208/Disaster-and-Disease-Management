using System;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Data;
using Disease_Disaster.Controllers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Views
{
	public partial class DiseaseManagementView : UserControl
	{
		private readonly DiseaseController _controller = new DiseaseController();
		private readonly AdministrativeController _adminController = new AdministrativeController();

		public DiseaseManagementView()
		{
			InitializeComponent();
			LoadInitData();
			LoadData();
		}

		private void LoadInitData()
		{
			try
			{
				// 1. Nạp danh sách Loại bệnh
				cbBenh.ItemsSource = _controller.GetLoaiBenh();

				// 2. Nạp danh sách TỈNH (Cấp 1) vào cbTinh
				// Hàm GetProvinces() lấy toàn bộ 63 tỉnh
				var dtTinh = _adminController.GetProvinces();

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

		// Sự kiện: Khi chọn Tỉnh -> Nạp danh sách Huyện tương ứng
		private void cbTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (cbTinh.SelectedValue != null)
				{
					int tinhId = (int)cbTinh.SelectedValue;

					// Nạp các huyện thuộc tỉnh đó vào cbDonVi
					cbDonVi.ItemsSource = _adminController.GetAllHuyen(tinhId).DefaultView;
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
				string keyword = txtSearch.Text.Trim();
				dgDisease.ItemsSource = _controller.GetOutbreaks(keyword);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
			}
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e) => LoadData();
		private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Kiểm tra: Phải chọn Tỉnh và Huyện
			if (cbDonVi.SelectedValue == null)
			{
				MessageBox.Show("Vui lòng chọn Địa điểm (Tỉnh -> Huyện) xảy ra dịch!");
				return;
			}
			if (cbBenh.SelectedValue == null)
			{
				MessageBox.Show("Vui lòng chọn Loại dịch bệnh!");
				return;
			}
			if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong < 0)
			{
				MessageBox.Show("Số lượng mắc bệnh phải là số nguyên dương!");
				return;
			}

			try
			{
				// DonViId lấy từ ComboBox Huyện (cbDonVi)
				bool success = _controller.AddOutbreak(
					(int)cbDonVi.SelectedValue,
					(int)cbBenh.SelectedValue,
					DateTime.Now,
					soLuong,
					txtNguyenNhan.Text,
					chkVaccine.IsChecked == true
				);

				if (success)
				{
					MessageBox.Show("Khai báo ổ dịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					LoadData();
					ClearForm();
				}
				else
				{
					MessageBox.Show("Lỗi: Không thể lưu vào cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi hệ thống: " + ex.Message);
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (dgDisease.SelectedItem is ODichHienThi item)
			{
				if (MessageBox.Show($"Bạn có chắc muốn xóa ổ dịch {item.TenBenh} tại {item.TenDonVi}?",
									"Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					if (_controller.DeleteOutbreak(item.Id))
					{
						LoadData();
						MessageBox.Show("Đã xóa ổ dịch.");
					}
					else
					{
						MessageBox.Show("Không thể xóa ổ dịch này.");
					}
				}
			}
		}

		private void btnMap_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var mapData = _controller.GetMapData();
				if (mapData.Count == 0)
				{
					MessageBox.Show("Hiện tại không có vùng dịch nào đang lây lan.", "An toàn");
					return;
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("⚠️ CẢNH BÁO CÁC VÙNG DỊCH ĐANG LÂY LAN:\n");
				foreach (var point in mapData)
				{
					string icon = point.LevelColor == "Red" ? "🔴" : "🟡";
					sb.AppendLine($"{icon} {point.TenDonVi}: {point.Info}");
				}
				MessageBox.Show(sb.ToString(), "Bản đồ dịch bệnh", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi tải dữ liệu bản đồ: " + ex.Message);
			}
		}

		private void btnAddDisease_Click(object sender, RoutedEventArgs e)
		{
			AddDiseaseWindow addWin = new AddDiseaseWindow();
			if (addWin.ShowDialog() == true)
			{
				try
				{
					cbBenh.ItemsSource = _controller.GetLoaiBenh();
					cbBenh.SelectedIndex = cbBenh.Items.Count - 1;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Lỗi làm mới danh sách: " + ex.Message);
				}
			}
		}

		private void ClearForm()
		{
			// Không reset cbTinh để tiện nhập tiếp
			cbDonVi.SelectedIndex = -1;
			cbBenh.SelectedIndex = -1;
			txtSoLuong.Text = "";
			txtNguyenNhan.Text = "";
			chkVaccine.IsChecked = false;
		}
	}
}