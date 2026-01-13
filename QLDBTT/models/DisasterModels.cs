using System;

namespace Disease_Disaster.Models
{
	public class DiemThienTai
	{
		public int Id { get; set; }
		public string TenDonVi { get; set; }
		public string TenLoaiThienTai { get; set; }
		public string CapHanhChinh { get; set; }
		public int MucDo { get; set; }
		public string GhiChu { get; set; }
		public DateTime NgayGhiNhan { get; set; }
		public string FileDinhKem { get; set; }

		public string TrangThaiFile
		{
			get
			{
				return string.IsNullOrEmpty(FileDinhKem) ? "Không" : "Có đính kèm";
			}
		}
	}
}