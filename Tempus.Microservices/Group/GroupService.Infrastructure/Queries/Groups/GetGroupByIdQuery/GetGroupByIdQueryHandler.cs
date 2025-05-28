using GroupService.Core.Entities;
using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;
using Tempus.Shared.Models.User;

namespace GroupService.Infrastructure.Queries.Groups.GetGroupByIdQuery;

public class GetGroupByIdQueryHandler(GroupServiceDbContext context, IHttpClientFactory httpClientFactory)
    : IRequestHandler<GetGroupByIdQuery, BaseResponse<GroupDetails>>
{
    private readonly GroupServiceDbContext context = context;
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("userservice-api");

    public async Task<BaseResponse<GroupDetails>> Handle(GetGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var group = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group == null)
        {
            return BaseResponse<GroupDetails>.NotFound("Group not found!");
        }

        var validator = ValidateGroup(request, group);

        if (validator.StatusCode != StatusCodes.Ok)
        {
            return validator;
        }

        var groupDetails = validator.Resource;

        groupDetails.Image = group.GroupPhoto?.Url;

        var userIds = await context.UserGroups
            .AsNoTracking()
            .Where(x => x.GroupId == group.Id)
            .Select(x => x.UserId)
            .ToListAsync();

        var userEmails = await GetUserEmails(userIds, request.Id);

        groupDetails.Members = userEmails;

        var result = BaseResponse<GroupDetails>.Ok(groupDetails);
        return result;
    }

    private BaseResponse<GroupDetails> ValidateGroup(GetGroupByIdQuery request, Group group)
    {
        if (request.UserId != group.OwnerId)
        {
            return BaseResponse<GroupDetails>.Forbbiden();
        }

        return BaseResponse<GroupDetails>.Ok(GenericMapper<Group, GroupDetails>.Map(group));
    }

    private async Task<List<UserEmail>> GetUserEmails(List<Guid> userIds, Guid requesterId)
    {
        var json = JsonConvert.SerializeObject(userIds);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Add("UserId", requesterId.ToString());

        var responseObject = await httpClient.PostAsync("/api/users/emails", content);

        var responseString = await responseObject.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<BaseResponse<List<UserEmail>>>(responseString);

        return response?.Resource;
    }
}