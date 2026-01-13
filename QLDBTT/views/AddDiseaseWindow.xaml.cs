using System.Windows;

namespace Disease_Disaster.Views
{
	public partial class AddDiseaseTypeWindow : Window
	{
		public string TenBenh { get; private set; }
		public string MoTa { get; private set; }

		public AddDiseaseTypeWindow()
		{
			InitializeComponent();
			txtTenBenh.Focus();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtTenBenh.Text))
			{
				MessageBox.Show("Vui lòng nhập tên bệnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			TenBenh = txtTenBenh.Text.Trim();
			MoTa = txtMoTa.Text.Trim();

			this.DialogResult = true; // Đánh dấu là người dùng ấn Lưu
			this.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}