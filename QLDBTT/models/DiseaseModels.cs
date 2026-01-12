using System;

namespace Disease_Disaster.Models
{
	// 3.1. Loại dịch bệnh
	public class LoaiDichBenh
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string MoTa { get; set; }
		// public string VatNuoi { get; set; } // Bỏ comment nếu DB có cột này
	}

	// 3.3 -> 3.6. Ổ Dịch (Model chính hiển thị)
	public class ODichHienThi
	{
		public int Id { get; set; }
		public int DonViId { get; set; }
		public int LoaiDichBenhId { get; set; }

		public string TenDonVi { get; set; }
		public string CapHanhChinh { get; set; }
		public string TenBenh { get; set; }

		public DateTime NgayPhatHien { get; set; }
		public int SoLuongMacBenh { get; set; }
		public int SoLuongTieuHuy { get; set; }
		public string TrangThai { get; set; }
		public string NguyenNhan { get; set; }
		public bool DaTiemPhong { get; set; }
		public string GhiChu { get; set; }

		// Thuộc tính hiển thị phụ trợ
		public string NgayHienThi => NgayPhatHien.ToString("dd/MM/yyyy");
		public string TrangThaiTiemPhong => DaTiemPhong ? "Đã tiêm bao vây" : "Chưa tiêm";
	}

	// 3.7. Dữ liệu bản đồ (Sửa lại class này để khớp với Controller)
	public class MapDataPoint
	{
		public string TenDonVi { get; set; }


		public string TenLoaiDichBenh { get; set; }

		public string LevelColor { get; set; } // Màu sắc cảnh báo (Red/Yellow)
		public string Info { get; set; }       // Thông tin hiển thị (Tooltip)
	}
}