using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Disease_Disaster.Helpers;
using Disease_Disaster.Models;

namespace Disease_Disaster.Controllers
{
	public class FacilityManagementController
	{
		private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
		public List<LoaiCoSo> GetFacilityTypes()
		{
			var list = new List<LoaiCoSo>();
			try
			{
				DataTable dt = _dbHelper.ExecuteQuery("SELECT * FROM LoaiCoSo");
				foreach (DataRow row in dt.Rows)
				{
					list.Add(new LoaiCoSo
					{
						Id = Convert.ToInt32(row["Id"]),
						Ten = row["Ten"].ToString()
					});
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Lỗi GetFacilityTypes: " + ex.Message);
			}
			return list;
		}

		// 1. Lấy danh sách cơ sở theo Tên Loại
		public List<CoSoHienThi> GetFacilitiesByType(string typeName, string keyword = "")
		{
			try
			{
				string query = @"SELECT * FROM ViewCoSoFull 
                                 WHERE TenLoaiCoSo = @Loai 
                                 AND (Ten LIKE @Key OR TenDonVi LIKE @Key)
                                 ORDER BY TenDonVi, Ten";

				SqlParameter[] param = {
					new SqlParameter("@Loai", typeName),
					new SqlParameter("@Key", "%" + keyword + "%")
				};

				var list = new List<CoSoHienThi>();
				DataTable dt = _dbHelper.ExecuteQuery(query, param);

				foreach (DataRow row in dt.Rows)
				{
					list.Add(new CoSoHienThi
					{
						Id = Convert.ToInt32(row["Id"]),
						Ten = row["Ten"].ToString(),
						QuyMo = row["QuyMo"].ToString(),
						SDT = row["SDT"].ToString(),
						LoaiCoSoId = Convert.ToInt32(row["LoaiCoSoId"]),
						TenLoaiCoSo = row["TenLoaiCoSo"].ToString(),
						DonViId = Convert.ToInt32(row["DonViId"]),
						TenDonVi = row["TenDonVi"].ToString(),
						CapHanhChinh = row.Table.Columns.Contains("CapHanhChinh") ? row["CapHanhChinh"].ToString() : ""
					});
				}
				return list;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Lỗi GetFacilitiesByType: " + ex.Message);
				return new List<CoSoHienThi>();
			}
		}

		// 2. Thống kê hộ chăn nuôi nhỏ lẻ
		public List<ThongKeHoChanNuoi> GetHouseholdStats()
		{
			try
			{
				string query = @"SELECT dv.Ten as TenDonVi, COUNT(cs.Id) as SoLuong, MAX(cs.QuyMo) as QuyMoTieuBieu
                                 FROM CoSo cs
                                 JOIN LoaiCoSo lcs ON cs.LoaiCoSoId = lcs.Id
                                 JOIN DonVi dv ON cs.DonViId = dv.Id
                                 WHERE lcs.Ten = N'Cá nhân chăn nuôi' 
                                 OR lcs.Ten = N'Hộ chăn nuôi'
                                 GROUP BY dv.Ten";

				var list = new List<ThongKeHoChanNuoi>();
				DataTable dt = _dbHelper.ExecuteQuery(query);

				foreach (DataRow row in dt.Rows)
				{
					list.Add(new ThongKeHoChanNuoi
					{
						TenDonVi = row["TenDonVi"].ToString(),
						SoLuongHo = Convert.ToInt32(row["SoLuong"]),
						QuyMoTieuBieu = row["QuyMoTieuBieu"].ToString()
					});
				}
				return list;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Lỗi Thống kê: " + ex.Message);
				return new List<ThongKeHoChanNuoi>();
			}
		}

		// 3. THÊM MỚI CƠ SỞ
		public bool AddFacility(string ten, int loaiCoSoId, int donViId, string quyMo, string sdt)
		{
			try
			{
				string query = "INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT) VALUES (@Ten, @LId, @DId, @QM, @SDT)";

				SqlParameter[] param = {
					new SqlParameter("@Ten", ten),
					new SqlParameter("@LId", loaiCoSoId),
					new SqlParameter("@DId", donViId),
					new SqlParameter("@QM", quyMo ?? ""),
					new SqlParameter("@SDT", sdt ?? "")
				};

				return _dbHelper.ExecuteNonQuery(query, param) > 0;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Lỗi Thêm mới: " + ex.Message);
				return false;
			}
		}

		// 4. XÓA CƠ SỞ
		public bool DeleteFacility(int id)
		{
			try
			{
				string query = "DELETE FROM CoSo WHERE Id = @Id";
				return _dbHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@Id", id) }) > 0;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Lỗi Xóa: " + ex.Message);
				return false;
			}
		}

		public List<ChiCucThuy> GetAllChiCuc()
		{
			var list = new List<ChiCucThuy>();
			try
			{
				DataTable dt = _dbHelper.ExecuteQuery("SELECT * FROM ChiCucThuy");
				foreach (DataRow row in dt.Rows)
				{
					list.Add(new ChiCucThuy { Id = (int)row["Id"], Ten = row["Ten"].ToString() });
				}
			}
			catch { }
			return list;
		}

		public List<GiayPhep> GetCertificates(int facilityId)
		{
			var list = new List<GiayPhep>();
			try
			{
				string query = "SELECT * FROM GiayPhep WHERE CoSoId = @Id ORDER BY NgayCap DESC";
				System.Data.DataTable dt = _dbHelper.ExecuteQuery(query, new[] { new System.Data.SqlClient.SqlParameter("@Id", facilityId) });

				foreach (System.Data.DataRow row in dt.Rows)
				{
					list.Add(new GiayPhep
					{
						Id = Convert.ToInt32(row["Id"]),
						SoGiayPhep = row["SoGiayPhep"].ToString(),
						NgayCap = Convert.ToDateTime(row["NgayCap"]),
						NgayHetHan = Convert.ToDateTime(row["NgayHetHan"]),
						CoSoId = Convert.ToInt32(row["CoSoId"]),
					});
				}
			}
			catch { }
			return list;
		}

		// Thêm giấy phép mới
		public bool AddCertificate(int facilityId, string soGiay, DateTime ngayCap, DateTime ngayHetHan)
		{
			try
			{
				string query = "INSERT INTO GiayPhep (SoGiayPhep, NgayCap, NgayHetHan, CoSoId) VALUES (@So, @Cap, @Het, @Id)";
				return _dbHelper.ExecuteNonQuery(query, new System.Data.SqlClient.SqlParameter[] {
			new System.Data.SqlClient.SqlParameter("@So", soGiay),
			new System.Data.SqlClient.SqlParameter("@Cap", ngayCap),
			new System.Data.SqlClient.SqlParameter("@Het", ngayHetHan),
			new System.Data.SqlClient.SqlParameter("@Id", facilityId)
		}) > 0;
			}
			catch { return false; }
		}
		public bool AddDieuKien(int giayPhepId, string tenDieuKien, string moTa)
		{
			try
			{
				string query = "INSERT INTO DieuKienChanNuoi (Ten, MoTa, GiayPhepId) VALUES (@Ten, @MoTa, @Id)";
				return _dbHelper.ExecuteNonQuery(query, new SqlParameter[] {
					new SqlParameter("@Ten", tenDieuKien),
					new SqlParameter("@MoTa", moTa),
					new SqlParameter("@Id", giayPhepId)
				}) > 0;
			}
			catch { return false; }
		}
		public DataTable SearchSafeZones(string keyword)
		{
			string query = @"
        SELECT 
            cs.Id, 
            cs.Ten, 
            dv.Ten as TenDonVi, 
            cs.QuyMo, 
            ISNULL(gp.SoGiayPhep, N'Chưa cấp') as SoGiayPhep,
            gp.NgayHetHan,
            CASE 
                WHEN gp.Id IS NULL THEN N'Chưa chứng nhận'
                WHEN gp.NgayHetHan < GETDATE() THEN N'Hết hạn'
                ELSE N'Đang hiệu lực'
            END as TrangThai,
            CASE 
                WHEN gp.Id IS NULL THEN 'Gray'
                WHEN gp.NgayHetHan < GETDATE() THEN 'Red'
                ELSE 'Green'
            END as ColorStatus
        FROM CoSo cs
        JOIN LoaiCoSo lcs ON cs.LoaiCoSoId = lcs.Id
        LEFT JOIN DonVi dv ON cs.DonViId = dv.Id
        LEFT JOIN GiayPhep gp ON gp.CoSoId = cs.Id
        WHERE lcs.Ten = N'Vùng chăn nuôi an toàn dịch bệnh'
        AND (cs.Ten LIKE @Key OR dv.Ten LIKE @Key OR gp.SoGiayPhep LIKE @Key)
        ORDER BY cs.Ten";

			return _dbHelper.ExecuteQuery(query, new[] { new System.Data.SqlClient.SqlParameter("@Key", "%" + keyword + "%") });
		}
	} 
} 