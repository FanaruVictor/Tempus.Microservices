using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Core.Entities;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure.IServices;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Commands.Registrations.Update;

public class
    UpdateRegistrationCommandHandler : IRequestHandler<UpdateRegistrationCommand, BaseResponse<RegistrationDetails>>
{
    private readonly ICloudinaryService _cloudinaryService;

    //private readonly IClientEventSender _clientEventSender;
    private readonly RegistrationServiceDbContext _context;

    public UpdateRegistrationCommandHandler(
            ICloudinaryService cloudinaryService
            , RegistrationServiceDbContext context)

        //    IClientEventSender clientEventSender)
    {
        _cloudinaryService = cloudinaryService;
        _context = context;

        //_clientEventSender = clientEventSender;
    }

    public async Task<BaseResponse<RegistrationDetails>> Handle(UpdateRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await _context.Registrations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if(entity == null)
            {
                return BaseResponse<RegistrationDetails>.NotFound($"Registration with Id: {request.Id} was not found");
            }

            if(request.GroupId.HasValue && entity.OwnerId != request.GroupId.Value &&
               request.GroupId.Value != Guid.Empty && entity.OwnerId != request.UserId)
            {
                return BaseResponse<RegistrationDetails>.Forbbiden();
            }

            entity = new Registration
            {
                Id = entity.Id,
                Description = request.Description,
                Content = request.Content,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = DateTime.UtcNow.Date,
                CategoryId = entity.CategoryId,
                OwnerId = entity.OwnerId
            };

            var images = ExtractImages(request.Content);

            var cloudinaryImages = await _cloudinaryService.UploadRegistrationImages(images);

            if(cloudinaryImages.Length > 0)
            {
                for(var i = 0; i < images.Count; i++)
                {
                    var image = images[i].Value;
                    var style = ExtractStyle(images[i].Value);
                    entity.Content = entity.Content?.Replace(image, CreateImage(cloudinaryImages[i], style));
                }
            }

            _context.Registrations.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            //await SendClientEvent(entity, request);

            //should put color in registrations table
            var detailedRegistration = GenericMapper<Registration, RegistrationDetails>.Map(entity);

            var result = BaseResponse<RegistrationDetails>.Ok(detailedRegistration);
            return result;
        }
        catch(Exception exception)
        {
            var result = BaseResponse<RegistrationDetails>.BadRequest(new List<string> {exception.Message});
            return result;
        }
    }

    private MatchCollection ExtractImages(string content)
    {
        var regex = new Regex(@"<img\s+[^>]*?src\s*=\s*""data:image\/\w+;base64,[^""]+""[^>]*?>");
        return regex.Matches(content);
    }

    private string CreateImage(string image, string style)
    {
        return$"<img src=\"{image}\" {style}/>";
    }

    private string ExtractStyle(string image)
    {
        var pattern =
            @"(?=.*style\s*=\s*""(?<style>[^""]*)"")(?=.*width\s*=\s*""(?<width>[^""]*)"")";

        var regex = new Regex(pattern);

        var match = regex.Match(image);

        if(match.Success)
        {
            // Extract the style and width attributes
            var style = match.Groups["style"].Value;
            var width = match.Groups["width"].Value;

            return$"style=\"{style}\" width=\"{width}\"";
        }

        return"";
    }

    /*private async Task SendClientEvent(Registration registration, UpdateRegistrationCommand request)
    {
        var category = await _categoryRepository.GetById(registration.CategoryId);

        var groupCategories = category.GroupCategories;

        foreach (var groupCategory in groupCategories)
        {
            var groupUsers = groupCategory.Group?.GroupUsers;

            if (groupUsers == null || groupUsers.Count == 0)
                continue;

            groupUsers = groupUsers.Where(x => x.UserId != request.UserId).ToList();
            foreach (var groupUser in groupUsers)
            {
                var registrationOverview = GenericMapper<Registration, RegistrationDetails>.Map(registration);
                registrationOverview.CategoryColor = category.Color;
                if (groupUser.UserId != request.UserId)
                    await _clientEventSender.SendRegistrationUpdated(registrationOverview, groupUser.GroupId,
                        groupUser.UserId.ToString());
            }
        }
    }*/
}