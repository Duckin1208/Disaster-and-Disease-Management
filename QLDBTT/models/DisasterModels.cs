using System;

namespace Disease_Disaster.Models
{
	// Model ánh xạ bảng DiemThienTai
	public class DiemThienTai
	{
		public int Id { get; set; }
		public string Ten { get; set; } // Tên điểm (nếu có trong DB, hoặc dùng để hiển thị mô tả)
		public int DonViId { get; set; }
		public int LoaiThienTaiId { get; set; }
		public int MucDo { get; set; } // Mức độ 1-5

		// Các thuộc tính mở rộng (Lấy từ ViewDiemThienTai để hiển thị)
		public string TenDonVi { get; set; }
		public string CapHanhChinh { get; set; }
		public string TenLoaiThienTai { get; set; } // "Trượt lở" hoặc "Lũ quét"
		public string TrangThaiFile { get; set; } // Dùng để hiển thị "Có file" hay "Không"
	}
}