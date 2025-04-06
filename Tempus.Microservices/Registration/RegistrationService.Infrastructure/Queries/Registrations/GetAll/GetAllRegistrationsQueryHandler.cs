using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RegistrationService.Core.Entities;
using RegistrationService.Data.Context;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetAll;

public class
    GetAllRegistrationsQueryHandler : IRequestHandler<GetAllRegistrationsQuery,
    BaseResponse<List<RegistrationDetails>>>
{
    private readonly string? _categoryServiceBaseUrl;
    private readonly RegistrationServiceDbContext _context;

    public GetAllRegistrationsQueryHandler(RegistrationServiceDbContext context, IConfiguration configuration)
    {
        _context = context;
        _categoryServiceBaseUrl = configuration["categoryServiceBaseURL"];
    }

    public async Task<BaseResponse<List<RegistrationDetails>>> Handle(GetAllRegistrationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var registrations = _context.Registrations
                .AsNoTracking()
                .Where(x => x.OwnerId == (request.GroupId.HasValue && request.GroupId.Value != Guid.Empty
                    ? request.GroupId
                    : request.UserId))
                .ToList();

            var registrationsOverview = registrations
                .Select(x =>
                {
                    var category = GetCategory(x.CategoryId, request.UserId, request.GroupId);

                    var currentRegistration = GenericMapper<Registration, RegistrationDetails>.Map(x);

                    currentRegistration.CategoryColor = category.Color;

                    return currentRegistration;
                })
                .ToList();

            var response = BaseResponse<List<RegistrationDetails>>.Ok(registrationsOverview);
            return response;
        }
        catch(Exception exception)
        {
            var response = BaseResponse<List<RegistrationDetails>>.BadRequest(new List<string> {exception.Message});
            return response;
        }
    }

    private BaseCategory GetCategory(Guid id, Guid userId, Guid? groupId)
    {
        var url = groupId.HasValue && groupId.Value != Guid.Empty
            ? $"{_categoryServiceBaseUrl}/{id}?groupId={groupId.Value}"
            : $"{_categoryServiceBaseUrl}/{id}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Add("UserId", userId.ToString());

        using var httpClient = new HttpClient();

        var responseObject = httpClient.Send(request);

        responseObject.EnsureSuccessStatusCode();

        var result = "";
        using(var stream = responseObject.Content.ReadAsStream())
        using(var reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }

        var response = JsonConvert.DeserializeObject<BaseResponse<BaseCategory>>(result);

        return response.Resource;
    }
}