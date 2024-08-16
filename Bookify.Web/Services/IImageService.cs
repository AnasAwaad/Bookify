namespace Bookify.Web.Services;

public interface IImageService
{
    public (bool isUploaded, string? errorMessage) UploadImage(IFormFile? imageFile, string imageName, string folderPath, bool hasThumbinal);

    public void DeleteImage(string fileName, string? imageThumbName = null);
}
