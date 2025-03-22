using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RegistrationService.Infrastructure.Commons;
using RegistrationService.Infrastructure.IServices;
using System.Text.RegularExpressions;

namespace RegistrationService.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig)
    {
        _cloudinaryConfig = cloudinaryConfig;

        var account = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<ImageUploadResult> Upload(IFormFile image)
    {
        if (image.Length < 0)
        {
            throw new Exception("Empty file");
        }

        await using var stream = image.OpenReadStream();
        return await UploadImage(stream, new Transformation().Width(200).Height(200).Crop("fill").Gravity("face"));
    }

    public async Task<string[]> UploadRegistrationImages(MatchCollection images)
    {
        var result = new string[] { };
        IEnumerable<byte[]?> photos = images.Select(x =>
        {
            if (x.Value.Contains("data:image/jpeg;base64") || x.Value.Contains("data:image/png;base64"))
            {
                var startIndex = x.Value.LastIndexOf(',') + 1;
                var length = x.Value.Length - startIndex - 2;
                var content = x.Value.Substring(startIndex, length);
                var index = content.IndexOf("\"");
                content = content.Substring(0, index);
                return Convert.FromBase64String(content);
            }

            return null;
        });

        foreach (var photo in photos)
        {
            if (photo != null)
            {
                var steam = new MemoryStream(photo);
                var uploadResult = await UploadImage(steam, null);
                result = result.Append(uploadResult.Url.ToString()).ToArray();
            }
        }

        return result;
    }

    private async Task<ImageUploadResult> UploadImage(Stream stream, Transformation? transformation)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(Guid.NewGuid().ToString(), stream),
        };

        if (transformation != null)
        {
            uploadParams.Transformation = transformation;
        }

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult;
    }
}