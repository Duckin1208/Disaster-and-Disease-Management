using System;
using System.Collections.Generic;

namespace Disease_Disaster.Models
{

	public class DonVi
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public int HanhChinhId { get; set; }
		public string TenHanhChinh { get; set; } // Tỉnh, Huyện, Xã
		public int? TrucThuocId { get; set; }    // ID đơn vị cha
		public int? ChiCucThuyId { get; set; }   // ID Chi cục quản lý
	}

	// Model dùng để hiển thị lên Giao diện (Dùng cho ViewDonVi trong SQL)
	public class DonViHienThi
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string TenHanhChinh { get; set; } // Ví dụ: Huyện
		public string CapHanhChinh { get; set; } // Ví dụ: Quận/Huyện
		public string TrucThuoc { get; set; }    // Tên đơn vị cha (VD: Lào Cai)
		public string ChiCucThuy { get; set; }  

		// Dùng để tạo cây thư mục (TreeView)
		public List<DonViHienThi> Children { get; set; } = new List<DonViHienThi>();
	}

	// Model cho Người dùng đầy đủ
	public class NguoiDungFull
	{
		public string TenDangNhap { get; set; }
		public string HoTen { get; set; }
		public string Email { get; set; }
		public string SDT { get; set; }
		public int QuyenId { get; set; }
		public string TenQuyen { get; set; }
		public int? HoSoId { get; set; }
	}
}