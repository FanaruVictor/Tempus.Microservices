using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace UserService.Infrastructure.IServices
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> Upload(IFormFile image);
        Task DestroyUsingUserId(Guid userId);
    }
}
