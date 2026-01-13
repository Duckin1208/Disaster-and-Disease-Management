using System;
using System.IO;
using System.Diagnostics;
using System.Windows; // Cần thiết để hiện MessageBox

namespace Disease_Disaster.Helpers
{
	public static class FileAttachmentHelper
	{
		// Tạo thư mục "Attachments" ngay tại nơi chạy phần mềm (bin/Debug)
		private static readonly string StorageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");

		static FileAttachmentHelper()
		{
			// Tự động tạo thư mục nếu chưa có
			if (!Directory.Exists(StorageFolder))
			{
				Directory.CreateDirectory(StorageFolder);
			}
		}

		// 1. Lưu file (Copy từ máy người dùng vào thư mục phần mềm)
		public static void SaveFile(int id, string sourcePath)
		{
			try
			{
				if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath)) return;

				// Xóa file cũ của ID này trước (nếu có) để tránh rác
				DeleteFile(id);

				// Lấy đuôi file gốc (ví dụ .pdf, .jpg)
				string extension = Path.GetExtension(sourcePath);

				// Đặt tên file mới là ID (ví dụ: 10.pdf)
				string destFile = Path.Combine(StorageFolder, $"{id}{extension}");

				File.Copy(sourcePath, destFile, true);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi lưu file: " + ex.Message);
			}
		}

		// 2. Xóa file đính kèm
		public static void DeleteFile(int id)
		{
			string filePath = FindFileById(id);
			if (!string.IsNullOrEmpty(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch { /* Bỏ qua lỗi nếu không xóa được */ }
			}
		}

		// 3. Kiểm tra xem bản ghi này có file không
		public static bool HasAttachment(int id)
		{
			return !string.IsNullOrEmpty(FindFileById(id));
		}

		// 4. [MỚI] Mở file bằng phần mềm mặc định của Windows
		public static void OpenFile(int id)
		{
			string filePath = FindFileById(id);

			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				try
				{
					// Sử dụng Process.Start với UseShellExecute = true để mở file
					Process.Start(new ProcessStartInfo
					{
						FileName = filePath,
						UseShellExecute = true
					});
				}
				catch (Exception ex)
				{
					MessageBox.Show("Không thể mở file này: " + ex.Message);
				}
			}
			else
			{
				MessageBox.Show("Bản ghi này chưa có file đính kèm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		// --- Hàm phụ trợ: Tìm file theo ID (bất kể đuôi file là gì) ---
		private static string FindFileById(int id)
		{
			// Tìm tất cả các file có tên bắt đầu bằng ID (ví dụ 10.*)
			var files = Directory.GetFiles(StorageFolder, $"{id}.*");

			// Nếu tìm thấy, trả về đường dẫn file đầu tiên
			if (files.Length > 0)
			{
				return files[0];
			}
			return null;
		}
	}
}