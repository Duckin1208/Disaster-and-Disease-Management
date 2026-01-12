using System;
using System.Data;
using System.Data.SqlClient;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Controllers
{
	public class CertificateController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();

		// 1. Lấy danh sách giấy phép (Trả về DataTable)
		public DataTable GetCertificates(int coSoId)
		{
			string query = "SELECT * FROM GiayPhep WHERE CoSoId = @Id ORDER BY NgayCap DESC";
			return _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Id", coSoId) });
		}

		// 2. Thêm giấy phép
		public bool AddCertificate(string soGP, DateTime ngayCap, DateTime ngayHH, int coSoId)
		{
			string query = "INSERT INTO GiayPhep (SoGiayPhep, NgayCap, NgayHetHan, CoSoId) VALUES (@So, @Cap, @HH, @Id)";
			SqlParameter[] param = {
				new SqlParameter("@So", soGP),
				new SqlParameter("@Cap", ngayCap),
				new SqlParameter("@HH", ngayHH),
				new SqlParameter("@Id", coSoId)
			};
			return _dbHelper.ExecuteNonQuery(query, param) > 0;
		}

		// 3. Xóa giấy phép (Xóa cả điều kiện con)
		public bool DeleteCertificate(int id)
		{
			try
			{
				// Xóa con trước
				_dbHelper.ExecuteNonQuery("DELETE FROM DieuKienChanNuoi WHERE GiayPhepId = @Id", new[] { new SqlParameter("@Id", id) });
				// Xóa cha sau
				return _dbHelper.ExecuteNonQuery("DELETE FROM GiayPhep WHERE Id = @Id", new[] { new SqlParameter("@Id", id) }) > 0;
			}
			catch { return false; }
		}

		// 4. Lấy danh sách điều kiện (Trả về DataTable)
		public DataTable GetConditions(int giayPhepId)
		{
			string query = "SELECT * FROM DieuKienChanNuoi WHERE GiayPhepId = @Id";
			return _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Id", giayPhepId) });
		}

		// 5. Thêm điều kiện
		public bool AddCondition(string ten, string moTa, int giayPhepId)
		{
			string query = "INSERT INTO DieuKienChanNuoi (Ten, MoTa, GiayPhepId) VALUES (@Ten, @MoTa, @GPId)";
			SqlParameter[] param = {
				new SqlParameter("@Ten", ten),
				new SqlParameter("@MoTa", moTa ?? ""), // Xử lý null
                new SqlParameter("@GPId", giayPhepId)
			};
			return _dbHelper.ExecuteNonQuery(query, param) > 0;
		}

		// 6. Xóa điều kiện
		public bool DeleteCondition(int id)
		{
			return _dbHelper.ExecuteNonQuery("DELETE FROM DieuKienChanNuoi WHERE Id = @Id", new[] { new SqlParameter("@Id", id) }) > 0;
		}
	}
}