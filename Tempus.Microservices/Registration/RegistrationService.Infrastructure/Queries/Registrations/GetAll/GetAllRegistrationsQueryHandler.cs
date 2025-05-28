using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegistrationService.Core.Entities;
using RegistrationService.Data.Context;
using System.Text;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetAll;

public class
    GetAllRegistrationsQueryHandler : IRequestHandler<GetAllRegistrationsQuery,
    BaseResponse<List<RegistrationDetails>>>
{
    private readonly HttpClient httpClient;
    private readonly RegistrationServiceDbContext _context;

    public GetAllRegistrationsQueryHandler(RegistrationServiceDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        httpClient = httpClientFactory.CreateClient("categoryservice-api");

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

            //problema de performanta si de call la category service
            var registrationsOverview = registrations
                .Select(x =>
                {
                    var category = GetCategory(x.CategoryId, request.UserId, request.GroupId).GetAwaiter().GetResult();

                    var currentRegistration = GenericMapper<Registration, RegistrationDetails>.Map(x);

                    currentRegistration.CategoryColor = category.Color;

                    return currentRegistration;
                })
                .ToList();

            var response = BaseResponse<List<RegistrationDetails>>.Ok(registrationsOverview);
            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<List<RegistrationDetails>>.BadRequest(new List<string> { exception.Message });
            return response;
        }
    }

    private async Task<BaseCategory> GetCategory(Guid id, Guid userId, Guid? groupId)
    {
        var url = groupId.HasValue && groupId.Value != Guid.Empty
            ? $"/api/categories/{id}?groupId={groupId.Value}"
            : $"/api/categories/{id}";

        if (httpClient.DefaultRequestHeaders.Contains("UserId"))
        {
            httpClient.DefaultRequestHeaders.Remove("UserId");
        }

        httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

        var responseObject = await httpClient.GetAsync(url);

        responseObject.EnsureSuccessStatusCode();

        var result = "";
        using (var stream = responseObject.Content.ReadAsStream())
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }

        var response = JsonConvert.DeserializeObject<BaseResponse<BaseCategory>>(result);

        return response.Resource;
    }
}