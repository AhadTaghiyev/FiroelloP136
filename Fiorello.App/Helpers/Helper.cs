using System;
namespace Fiorello.App.Helpers
{
	public class Helper
	{
		public static bool IsImage(IFormFile file)
		{
			return file.ContentType.Contains("image");
		}
		public static bool IsSizeOk(IFormFile file,int size)
		{
			return file.Length / 1024 / 1024 <= size;
		}

		public static void RemoveImage(string root,string path,string image)
		{
			string fullPath = Path.Combine(root,path,image);
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
		}
	}
}

