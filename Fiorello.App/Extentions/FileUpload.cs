using System;
using Fiorello.Core.Entities;

namespace Fiorello.App.Extentions
{
	public static class FileUpload
	{
		public static string CreateImage(this IFormFile file,string root,string path)
		{
            string FileName = Guid.NewGuid().ToString() + file.FileName;
            string FullPath = Path.Combine(root,path,FileName);
            using (FileStream fileStream = new FileStream(FullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return FileName;
        }
	}
}

