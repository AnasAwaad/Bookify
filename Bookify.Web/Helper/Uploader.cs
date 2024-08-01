using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Helper;

public static class Uploader
{
	
    public static string UploadImage(IFormFile file,string path,IWebHostEnvironment webHostEnvironment)
	{
		string FileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
		var finalPath = Path.Combine(webHostEnvironment.WebRootPath, path, FileName);
		using (var Stream = new FileStream(finalPath, FileMode.Create))
		file.CopyTo(Stream);

		return $"/{path}/{FileName}";
	}
	
	public static string UploadImageThumb(IFormFile file,string path,IWebHostEnvironment webHostEnvironment)
	{
		string FileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
		var finalPath = Path.Combine(webHostEnvironment.WebRootPath,path, FileName);
		using var image = Image.Load(file.OpenReadStream());
		var ratio = image.Width / 200.0;
		var height = image.Height / ratio;
		image.Mutate(img => img.Resize(200,(int)height));
		image.Save(finalPath);

		return $"/{path}/{FileName}";
	}
	public static void RemoveFile(string fileName,IWebHostEnvironment webHostEnvironment)
	{
		var oldImagePath = $"{webHostEnvironment.WebRootPath}{fileName}";
		if (System.IO.File.Exists(oldImagePath))
		{
			System.IO.File.Delete(oldImagePath);
		}
	}
}
