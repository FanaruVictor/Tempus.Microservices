using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GroupService.Core.Entities;
using GroupService.Data.Context;
using GroupService.Infrastructure.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GroupService.Infrastructure.Services.Cloudynary;

public class CloudinaryService : ICloudinaryService
{
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private readonly Cloudinary _cloudinary;
    private readonly GroupServiceDbContext _context;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig, GroupServiceDbContext context)
    {
        _cloudinaryConfig = cloudinaryConfig;
        _context = context;

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

    public async Task DestroyUsingGroupId(Guid groupId)
    {
        var photo = await _context
            .Photos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GroupId == groupId);

        if (photo == null)
        {
            throw new Exception("Photo not found");
        }

        await Destroy(photo);
    }

    public async Task Destroy(Photo photo)
    {
        var destroyParams = new DeletionParams(photo.PublicId)
        {
            ResourceType = ResourceType.Image,
        };

        await _cloudinary.DestroyAsync(destroyParams);
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