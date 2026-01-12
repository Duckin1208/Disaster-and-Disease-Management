using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Disease_Disaster.Helpers;

namespace Disease_Disaster.Controllers
{
	public class AdministrativeController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();


		// 1. Lấy danh sách Tỉnh (Dùng cho cả AdminView và các View khác)
		public DataTable GetProvinces()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM DonVi WHERE HanhChinhId = 1 ORDER BY Ten");
		}

		public DataTable GetTinh() => GetProvinces();

		public DataTable GetAllHuyen(int provinceId)
		{
			string query = "SELECT Id, Ten FROM DonVi WHERE TrucThuocId = @Id AND HanhChinhId = 2 ORDER BY Ten";
			return _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Id", provinceId) });
		}

		public DataTable GetAllXa(int districtId)
		{
			string query = "SELECT Id, Ten FROM DonVi WHERE TrucThuocId = @Id AND HanhChinhId = 3 ORDER BY Ten";
			return _dbHelper.ExecuteQuery(query, new[] { new SqlParameter("@Id", districtId) });
		}


		
		public DataTable GetLevels()
		{
			return _dbHelper.ExecuteQuery("SELECT Id, Ten FROM HanhChinh ORDER BY Id");
		}

		
		public DataTable Search(int? provinceId, int? levelId, string keyword)
		{
			string query = @"SELECT dv.Id, dv.Ten, hc.Ten as Cap, p.Ten as TrucThuoc, ct.Ten as ChiCucThuY 
                             FROM DonVi dv
                             LEFT JOIN HanhChinh hc ON dv.HanhChinhId = hc.Id
                             LEFT JOIN DonVi p ON dv.TrucThuocId = p.Id
                             LEFT JOIN ChiCucThuy ct ON dv.ChiCucThuyId = ct.Id
                             WHERE 1=1 ";

			if (!string.IsNullOrEmpty(keyword))
			{
				query += " AND dv.Ten LIKE @Key ";
			}

			if (levelId.HasValue)
			{
				query += " AND dv.HanhChinhId = @Level ";
			}

			
			if (provinceId.HasValue)
			{
				// Nếu tìm XÃ (Cấp 3) trong TỈNH -> Lấy các xã có cha (Huyện) thuộc Tỉnh
				if (levelId == 3)
				{
					query += " AND dv.TrucThuocId IN (SELECT Id FROM DonVi WHERE TrucThuocId = @ProvId) ";
				}
				// Nếu tìm HUYỆN (Cấp 2) trong TỈNH
				else if (levelId == 2)
				{
					query += " AND dv.TrucThuocId = @ProvId ";
				}
				// Nếu tìm chính TỈNH đó
				else if (levelId == 1)
				{
					query += " AND dv.Id = @ProvId ";
				}
				// Nếu không chọn cấp -> Lấy hết (Tỉnh đó OR Con nó OR Cháu nó)
				else
				{
					query += @" AND (dv.Id = @ProvId 
                                     OR dv.TrucThuocId = @ProvId 
                                     OR dv.TrucThuocId IN (SELECT Id FROM DonVi WHERE TrucThuocId = @ProvId)) ";
				}
			}

			query += " ORDER BY dv.HanhChinhId, dv.Ten";

			var parameters = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(keyword)) parameters.Add(new SqlParameter("@Key", "%" + keyword + "%"));
			if (levelId.HasValue) parameters.Add(new SqlParameter("@Level", levelId.Value));
			if (provinceId.HasValue) parameters.Add(new SqlParameter("@ProvId", provinceId.Value));

			return _dbHelper.ExecuteQuery(query, parameters.ToArray());
		}

		// Xóa đơn vị
		public bool Delete(int id)
		{
			try
			{
				// Xóa con trước (nếu có)
				_dbHelper.ExecuteNonQuery("DELETE FROM DonVi WHERE TrucThuocId = @Id", new[] { new SqlParameter("@Id", id) });
				// Xóa chính nó
				return _dbHelper.ExecuteNonQuery("DELETE FROM DonVi WHERE Id = @Id", new[] { new SqlParameter("@Id", id) }) > 0;
			}
			catch { return false; }
		}

		// Thêm mới
		public bool Add(string ten, int cap, int? trucThuocId)
		{
			string query = "INSERT INTO DonVi (Ten, HanhChinhId, TrucThuocId) VALUES (@Ten, @Cap, @Par)";
			return _dbHelper.ExecuteNonQuery(query, new[] {
				 new SqlParameter("@Ten", ten),
				 new SqlParameter("@Cap", cap),
				 new SqlParameter("@Par", trucThuocId.HasValue ? (object)trucThuocId.Value : DBNull.Value)
			 }) > 0;
		}

		// Alias cho AddDonVi nếu code cũ dùng tên này
		public bool AddDonVi(string ten, int cap, int? trucThuocId) => Add(ten, cap, trucThuocId);

		// Alias cho DeleteDonVi nếu code cũ dùng tên này
		public bool DeleteDonVi(int id) => Delete(id);
	}
}