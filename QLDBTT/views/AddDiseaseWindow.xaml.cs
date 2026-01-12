using System.Windows;
using Disease_Disaster.Controllers;

namespace Disease_Disaster.Views
{
	public partial class AddDiseaseWindow : Window
	{
		private readonly DiseaseController _controller = new DiseaseController();

		public AddDiseaseWindow()
		{
			InitializeComponent();
			txtTen.Focus(); // Tự động trỏ chuột vào ô nhập tên
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtTen.Text))
			{
				MessageBox.Show("Vui lòng nhập tên dịch bệnh!");
				return;
			}

			if (_controller.AddLoaiBenh(txtTen.Text.Trim(), txtMoTa.Text.Trim()))
			{
				MessageBox.Show("Thêm thành công!");
				DialogResult = true; // Báo cho cửa sổ cha biết là đã thêm xong
				Close();
			}
			else
			{
				MessageBox.Show("Lỗi: Có thể tên bệnh đã tồn tại.");
			}
		}
	}
}