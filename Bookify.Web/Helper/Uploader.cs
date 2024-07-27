using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Helper;

public static class Uploader
{
	
    public static string UploadFile(IFormFile file,string folder,IWebHostEnvironment webHostEnvironment)
	{
		string FileName = Guid.NewGuid() + Path.GetFileName(file.FileName);
		var finalPath = Path.Combine(webHostEnvironment.WebRootPath, "images", folder, FileName);
		using (var Stream = new FileStream(finalPath, FileMode.Create))
		file.CopyTo(Stream);

		return Path.Combine(FileName);
	}
}
