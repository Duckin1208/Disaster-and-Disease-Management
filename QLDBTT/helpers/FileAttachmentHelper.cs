using System;
using System.IO;

namespace Disease_Disaster.Helpers
{
	public static class FileAttachmentHelper
	{
		// Đường dẫn thư mục lưu file: bin/Debug/Reports/
		private static readonly string StorageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

		// Tạo thư mục nếu chưa có
		public static void EnsureFolderExists()
		{
			if (!Directory.Exists(StorageFolder))
			{
				Directory.CreateDirectory(StorageFolder);
			}
		}

		// Lưu file vào hệ thống
		// disasterId: ID của điểm thiên tai
		// sourceFilePath: Đường dẫn file gốc người dùng chọn
		public static void SaveFile(int disasterId, string sourceFilePath)
		{
			EnsureFolderExists();

			string extension = Path.GetExtension(sourceFilePath);
			// Quy tắc đặt tên: Report_{ID}.{duôi_file} (Ví dụ: Report_15.pdf)
			string destFileName = $"Report_{disasterId}{extension}";
			string destPath = Path.Combine(StorageFolder, destFileName);

			// Copy file vào thư mục hệ thống (Ghi đè nếu đã tồn tại)
			File.Copy(sourceFilePath, destPath, true);
		}

		// Lấy đường dẫn file của một ID (nếu có)
		public static string GetFilePath(int disasterId)
		{
			EnsureFolderExists();

			// Tìm tất cả các file bắt đầu bằng Report_{ID}
			string searchPattern = $"Report_{disasterId}.*";
			string[] files = Directory.GetFiles(StorageFolder, searchPattern);

			if (files.Length > 0)
			{
				return files[0]; // Trả về file đầu tiên tìm thấy
			}
			return null; // Không có file
		}

		// Kiểm tra xem ID này có file chưa
		public static bool HasAttachment(int disasterId)
		{
			return GetFilePath(disasterId) != null;
		}

		// Xóa file đính kèm
		public static void DeleteFile(int disasterId)
		{
			string path = GetFilePath(disasterId);
			if (path != null && File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}