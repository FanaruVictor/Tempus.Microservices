using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace GroupService.Infrastructure.Services.Cloudynary;

public interface ICloudinaryService
{
    Task<ImageUploadResult> Upload(IFormFile image);
    Task DestroyUsingGroupId(Guid groupId);
}