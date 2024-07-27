﻿using Microsoft.AspNetCore.Hosting;
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

		return FileName;
	}
	public static void RemoveFile(string filePath,IWebHostEnvironment webHostEnvironment)
	{
		var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath,"images","Book", filePath);
		if (System.IO.File.Exists(oldImagePath))
		{
			System.IO.File.Delete(oldImagePath);
		}
	}
}
