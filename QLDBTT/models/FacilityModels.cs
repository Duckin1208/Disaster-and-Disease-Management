using System;

namespace Disease_Disaster.Models
{
	// 1. Dùng cho ComboBox Loại Cơ Sở (Nạp danh sách loại hình)
	public class LoaiCoSo
	{
		public int Id { get; set; }
		public string Ten { get; set; }
	}

	// 2. Model hiển thị danh sách cơ sở (Map với ViewCoSoFull)
	public class CoSoHienThi
	{
		// Thông tin chính
		public int Id { get; set; }
		public string Ten { get; set; }
		public string QuyMo { get; set; }
		public string SDT { get; set; }

		// Khóa ngoại (Dùng để set giá trị mặc định khi Sửa/Edit)
		public int LoaiCoSoId { get; set; }
		public int DonViId { get; set; }

		// Thông tin hiển thị (Lấy từ bảng JOIN trong SQL View)
		public string TenLoaiCoSo { get; set; } // Ví dụ: Đại lý thú y
		public string TenDonVi { get; set; }    // Ví dụ: Xã A
		public string CapHanhChinh { get; set; } // Ví dụ: Xã/Huyện
	}

	// 3. Dùng cho Chi Cục Thú Y (4.1, 4.2)
	public class ChiCucThuy
	{
		public int Id { get; set; }
		public string Ten { get; set; }
	}

	// 4. Giấy phép (Dùng cho CertificateController)
	public class GiayPhep
	{
		public int Id { get; set; }
		public string SoGiayPhep { get; set; }
		public DateTime NgayCap { get; set; }
		public DateTime NgayHetHan { get; set; }

		public int CoSoId { get; set; }
		public string TenCoSo { get; set; } // Hiển thị tên cơ sở thay vì số ID

		// Thuộc tính phụ trợ (Helper) hiển thị trạng thái trên DataGrid
		public string TrangThai => NgayHetHan < DateTime.Now ? "Đã hết hạn" : "Còn hiệu lực";
		public string NgayHetHanString => NgayHetHan.ToString("dd/MM/yyyy");
	}

	// 5. Điều kiện chăn nuôi
	public class DieuKienChanNuoi
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string MoTa { get; set; }
		public int GiayPhepId { get; set; }
	}

	// 6. Thống kê (Dùng cho hàm GetHouseholdStats)
	public class ThongKeHoChanNuoi
	{
		public string TenDonVi { get; set; }
		public int SoLuongHo { get; set; }
		public string QuyMoTieuBieu { get; set; }
	}
}