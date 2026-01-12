using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows; 

namespace Disease_Disaster.Helpers
{
	public class DatabaseHelper
	{
		// Chuỗi kết nối đến Database QLDBTT
		private readonly string _connectionString = "Server=DESKTOP-U1PHCCA;Database=QLDBTT;Trusted_Connection=True;";

		public SqlConnection GetConnection()
		{
			return new SqlConnection(_connectionString);
		}

		/// <summary>
		/// Thực thi truy vấn SELECT và trả về DataTable (Dùng cho các View như ViewHoSo, ViewDonVi)
		/// </summary>
		public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
		{
			using (var connection = GetConnection())
			{
				try
				{
					var command = new SqlCommand(query, connection);
					if (parameters != null) command.Parameters.AddRange(parameters);

					var adapter = new SqlDataAdapter(command);
					var dataTable = new DataTable();

					connection.Open();
					adapter.Fill(dataTable); // Đây là nơi thường xảy ra lỗi "Invalid column name"
					return dataTable;
				}
				catch (SqlException ex)
				{
					// Hiển thị thông báo chi tiết lỗi SQL để bạn dễ sửa View/Table
					MessageBox.Show($"Lỗi truy vấn SQL: {ex.Message}\nCâu lệnh: {query}", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
					return new DataTable();
				}
			}
		}

		/// <summary>
		/// Thực thi INSERT, UPDATE, DELETE (Dùng cho AddDonVi, UpdateUser...)
		/// </summary>
		public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
		{
			using (var connection = GetConnection())
			{
				try
				{
					var command = new SqlCommand(query, connection);
					if (parameters != null) command.Parameters.AddRange(parameters);

					connection.Open();
					return command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Lỗi thực thi SQL: {ex.Message}", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
					return -1;
				}
			}
		}

		/// <summary>
		/// Thực thi lấy một giá trị duy nhất (Dùng để lấy SCOPE_IDENTITY() khi thêm Hồ sơ mới)
		/// </summary>
		public object ExecuteScalar(string query, SqlParameter[] parameters = null)
		{
			using (var connection = GetConnection())
			{
				try
				{
					var command = new SqlCommand(query, connection);
					if (parameters != null) command.Parameters.AddRange(parameters);

					connection.Open();
					return command.ExecuteScalar();
				}
				catch (SqlException ex)
				{
					MessageBox.Show($"Lỗi lấy dữ liệu đơn: {ex.Message}", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
					return null;
				}
			}
		}
	}
}