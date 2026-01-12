using System;

namespace Disease_Disaster.Models
{
	
	public class ThongKeTongHop
	{
		public int TongNguoiDung { get; set; }
		public int TongTacDong { get; set; }
		public int TongTruyCap { get; set; }
	}
	
	public class NhatKyHeThong
	{
		public int Id { get; set; }
		public string TaiKhoan { get; set; }
		public string HoTen { get; set; }
		public string HanhDong { get; set; }
		public string NoiDung { get; set; }
		public DateTime ThoiGian { get; set; }
	}
}
