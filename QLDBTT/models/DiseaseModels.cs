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

	// 3.3 -> 3.6. Ổ Dịch (Model chính hiển thị

		// Model cho Ổ Dịch
	public class ODichModel
		{
			public int Id { get; set; }
			public string TenODich { get; set; } // [MỚI] Tên ổ dịch
			public int DonViId { get; set; }
			public string TenDonVi { get; set; }
			public string TenBenh { get; set; }
			public DateTime NgayPhatHien { get; set; }
			public int SoLuongMacBenh { get; set; }
			public string TrangThai { get; set; }
			public string NguyenNhan { get; set; }
			public string ChanDoan { get; set; }
			public bool DaTiemPhong { get; set; }
		}

	// Model cho Tiêm Phòng [MỚI]
	public class TiemPhongModel
	{
		public int Id { get; set; }
		public string TenDotTiem { get; set; }
		public string TenBenh { get; set; }
		public string TenODich { get; set; } 
		public int ODichId { get; set; } 
		public DateTime NgayTiem { get; set; }
		public string LoaiVaccine { get; set; }
		public int SoLuong { get; set; }
		public string NguoiThucHien { get; set; }
	}


	
	public class MapDataPoint
	{
		public string TenDonVi { get; set; }


		public string TenLoaiDichBenh { get; set; }

		public string LevelColor { get; set; } 
		public string Info { get; set; } 
	}
}