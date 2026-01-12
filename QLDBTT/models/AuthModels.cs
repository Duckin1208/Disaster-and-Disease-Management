using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Disease_Disaster.Models
{
	// Bảng: HoSo
	public class HoSo
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string SDT { get; set; }
		public string Email { get; set; }
		public string Ext { get; set; }
	}

	// Bảng: Quyen
	public class Quyen
	{
		public int Id { get; set; }
		public string Ten { get; set; } // Lập trình viên, Admin, Staff
		public string Ext { get; set; } // Mã quyền (Developer, Admin...)
	}

	public class TaiKhoan
	{
		public int Id { get; set; }
		public string TenDangNhap { get; set; }
		public string HoTen { get; set; }
		public string MatKhau { get; set; }
		public int QuyenId { get; set; }
		public string TenQuyen { get; set; }
		public string Email { get; set; }
		public string SDT { get; set; }

		public int? HoSoId { get; set; }
	}

	// Bảng: LichSuThaoTac
	public class LichSuThaoTac
	{
		public int Id { get; set; }
		public string TaiKhoanTen { get; set; }
		public string ThaoTac { get; set; }
		public string GiaTriCu { get; set; }
		public string GiaTriMoi { get; set; }
		public DateTime ThoiGianDangNhap { get; set; }

		// Mở rộng (từ ViewLichSuThaoTac)
		public string HoTenNguoiDung { get; set; }
		public string TenQuyen { get; set; }
	}
}