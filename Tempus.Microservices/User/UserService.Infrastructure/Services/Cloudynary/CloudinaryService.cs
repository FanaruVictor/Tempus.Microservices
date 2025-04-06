using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tempus.Shared.Commons;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.IServices;

namespace UserService.Infrastructure.Services.Cloudynary;

public class CloudinaryService : ICloudinaryService
{
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private readonly Cloudinary _cloudinary;
    private readonly UserServiceDbContext _context;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig, UserServiceDbContext context)
    {
        _cloudinaryConfig = cloudinaryConfig;

        var account = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _context = context;
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



    public async Task DestroyUsingUserId(Guid userId)
    {
        var photo = await _context.Photos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new Exception("Photo not found");

        if (photo.PublicId != "")
        {
            await Destroy(photo);
        }
    }

    private async Task Destroy(Photo photo)
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