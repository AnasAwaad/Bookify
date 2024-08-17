namespace Bookify.Web.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IList<string> _allowedExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
    private readonly int _allowedSize = 1048576;


    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }


    public (bool isUploaded, string? errorMessage) UploadImage(IFormFile? imageFile, string imageName, string folderPath, bool hasThumbinal)
    {
        if (imageFile is null)
            return (isUploaded: false, errorMessage: "The Image field is required.");

        // check extension of image
        if (!_allowedExtensions.Contains(Path.GetExtension(imageFile.FileName).ToLower()))
            return (isUploaded: false, errorMessage: Errors.AllowedExtensions);

        // check size of image
        if (imageFile.Length > _allowedSize)
            return (isUploaded: false, errorMessage: Errors.AllowedSize);


        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath, imageName);

        using (var Stream = new FileStream(imagePath, FileMode.Create))
            imageFile.CopyTo(Stream);


        if (hasThumbinal)
        {
            var thumbPath = $"{_webHostEnvironment.WebRootPath}/{folderPath}/thumb/{imageName}";

            using var image = Image.Load(imageFile.OpenReadStream());
            var ratio = image.Width / 200.0;
            var height = image.Height / ratio;
            image.Mutate(img => img.Resize(200, (int)height));
            image.Save(thumbPath);
        }
        return (isUploaded: true, errorMessage: null);
    }

    public void DeleteImage(string fileName, string? imageThumbName = null)
    {
        var ImagePath = $"{_webHostEnvironment.WebRootPath}{fileName}";
        if (System.IO.File.Exists(ImagePath))
        {
            System.IO.File.Delete(ImagePath);
        }

        if (imageThumbName is not null)
        {
            var ImageThumbPath = $"{_webHostEnvironment.WebRootPath}{imageThumbName}";
            if (System.IO.File.Exists(ImageThumbPath))
            {
                System.IO.File.Delete(ImageThumbPath);
            }
        }
    }

}
