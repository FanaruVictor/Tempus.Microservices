using MediatR;
using RegistrationService.Core.Commons;
using RegistrationService.Core.Entities;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure.Commons;
using RegistrationService.Infrastructure.IServices;
using RegistrationService.Infrastructure.Models;
using System.Text.RegularExpressions;

namespace RegistrationService.Infrastructure.Commands.Registrations.Create;

public class
    CreateRegistrationCommandHandler : IRequestHandler<CreateRegistrationCommand, BaseResponse<RegistrationDetails>>
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly RegistrationServiceDbContext _context;

    public CreateRegistrationCommandHandler(ICloudinaryService cloudinaryService, RegistrationServiceDbContext context)
    {
        _cloudinaryService = cloudinaryService;
        _context = context;
    }

    public async Task<BaseResponse<RegistrationDetails>> Handle(CreateRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();


            //should get the category from categoryService

            Category category = null;

            if (category == null)
            {
                return BaseResponse<RegistrationDetails>.BadRequest(new List<string>
                    { $"Category with Id: {request.CategoryId} not found" });
            }

            var entity = new Registration
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow.Date,
                LastUpdatedAt = DateTime.UtcNow.Date,
                CategoryId = request.CategoryId,
                OwnerId = request.GroupId.HasValue ? request.GroupId.Value : request.UserId
            };

            var images = ExtractImages(request.Content);

            var cloudinaryImages = await _cloudinaryService.UploadRegistrationImages(images);

            if (cloudinaryImages.Length > 0)
            {
                for (var i = 0; i < images.Count; i++)
                {
                    var image = images[i].Value;
                    var style = ExtractStyle(images[i].Value);
                    entity.Content = entity.Content?.Replace(image, CreateImage(cloudinaryImages[i], style));
                }
            }

            await _context.Registrations.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            var registrationOverview = GenericMapper<Registration, RegistrationDetails>.Map(entity);
            registrationOverview.CategoryColor = category.Color;

            //await SendEvent(registrationOverview, category);


            var result = BaseResponse<RegistrationDetails>.Ok(registrationOverview);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<RegistrationDetails>.BadRequest(new List<string> { exception.Message });
            return result;
        }
    }

    private MatchCollection ExtractImages(string content)
    {
        var regex = new Regex(@"<img.*?src=""(.*?)"".*?>");
        return regex.Matches(content);
    }

    private string CreateImage(string image, string style)
    {
        return $"<img src=\"{image}\" {style}/>";
    }

    private string ExtractStyle(string image)
    {
        string pattern =
            @"<img\s+[^>]*?style\s*=\s*""(?<style>[^""]*)""\s*[^>]*?width\s*=\s*""(?<width>[^""]*)""[^>]*?>";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(image);

        if (match.Success)
        {
            // Extract the style and width attributes
            string style = match.Groups["style"].Value;
            string width = match.Groups["width"].Value;

            return $"style=\"{style}\" width=\"{width}\"";
        }

        return "";
    }

    /*private async Task SendEvent(RegistrationDetails registration, Category category)
    {
        var groupCategories = category.GroupCategories;

        foreach (var groupCategory in groupCategories)
        {
            var groupUsers = groupCategory.Group?.GroupUsers;

            if (groupUsers == null || groupUsers.Count == 0)
                continue;

            foreach (var groupUser in groupUsers)
            {
                await _clientEventSender.SendRegistrationCreatedEventAsync(registration, groupUser.UserId.ToString());
            }
        }
    }*/
}