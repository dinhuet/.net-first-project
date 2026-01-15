using BlogApp.Application.MiddleWare;
using BlogApp.Infrastructure.ExternalServices.Interface;
using BlogApp.Infrastructure.ExternalServices.Minio;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BlogApp.Infrastructure.ExternalServices.Impl;

public class UploadService :  IUploadService
{
    private readonly Cloudinary _cloudinary;
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;
    
    private static readonly string[] AllowedExtensions =
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };
    
    public UploadService(Cloudinary cloudinary, IOptions<MinioOptions> options, IMinioClient minioClient)
    {
        _minioClient = minioClient;
        _options = options.Value;
        _cloudinary = cloudinary;
    }
    
    public void ValidateImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new AppException(ErrorCode.FileIsEmpty);

        // Size (ví dụ: 5MB)
        if (file.Length > 5 * 1024 * 1024)
            throw new AppException(ErrorCode.ImageTooLarge);

        // Extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new AppException(ErrorCode.ImageNotAllowed);

        // Content-Type
        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new AppException(ErrorCode.ContentTypeImageNotAllowed);
    }

    // upload = cloudinary
    /*public async Task<string?> UploadImageAsync(IFormFile file)
    {
        ValidateImage(file);

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "products",
            Transformation = new Transformation()
                .Width(800)
                .Height(800)
                .Crop("limit")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }*/

    // presigned url (1h)
    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        ValidateImage(file);
        
        
        var bucket = _options.Buckets["Blogs"];
        
        // file name
        var extension = Path.GetExtension(file.FileName);
        var datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var fileName = $"{DateTime.UtcNow:HHmmssfff}{extension}";

        var objectName = $"products/{datePath}/{fileName}";


        await using var stream = file.OpenReadStream();

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
        );
        
        // presigned url
        /*
        var url = await _minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket("blog-images")
                .WithObject(objectName)
                .WithExpiry(60 * 60) // 1 giờ
        );
        */

        var endpoint = _options.Endpoint;

        return $"http://{endpoint}/{bucket}/{objectName}";
    }
}