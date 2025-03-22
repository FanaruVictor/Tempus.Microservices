using System.Text.RegularExpressions;

namespace RegistrationService.Infrastructure.IServices;

public interface ICloudinaryService
{
    Task<string[]> UploadRegistrationImages(MatchCollection images);
}