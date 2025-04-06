using System.Text;
using GroupService.Core.Entities;
using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Queries.Groups.GetAllGroupsQuery;

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, BaseResponse<List<GroupOverview>>>
{
    private readonly GroupServiceDbContext context;
    private readonly string? userServiceBaseUrl;

    public GetAllGroupsQueryHandler(GroupServiceDbContext context, IConfiguration configuration)
    {
        this.context = context;
        userServiceBaseUrl = configuration["userServiceBaseURL"];
    }

    public async Task<BaseResponse<List<GroupOverview>>> Handle(GetAllGroupsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userGroups = await context.UserGroups
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            var groupIds = userGroups.Select(x => x.GroupId).ToList();

            var groups = await context.Groups
                .AsNoTracking()
                .Where(x => groupIds.Contains(x.Id))
                .ToListAsync();

            var groupsOverview = groups.Select(x =>
            {
                var groupImage = x.GroupPhoto?.Url;

                return new GroupOverview
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = groupImage,
                    UserCount = context.UserGroups.Count(y => y.GroupId == x.Id),
                    CreatedAt = x.CreatedAt,
                    OwnerId = x.OwnerId
                };
            }).ToList();


            foreach(var group in groupsOverview)
            {
                var currentUserPhotos = await GetUserPhotos(userGroups, request.UserId);

                group.UserPhotos = currentUserPhotos ?? new List<string>();
            }

            return BaseResponse<List<GroupOverview>>.Ok(groupsOverview);
        }
        catch(Exception exception)
        {
            return BaseResponse<List<GroupOverview>>.BadRequest(new List<string> {exception.Message});
        }
    }

    private async Task<List<string>?> GetUserPhotos(List<UserGroup> userGroups, Guid? userId)
    {
        var userIds = userGroups.Select(x => x.UserId).ToList();

        var json = JsonConvert.SerializeObject(userIds);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{userServiceBaseUrl}/photos");

        request.Headers.Add("UserId", userId.ToString());

        request.Content = content;

        using var httpClient = new HttpClient();

        var responseObject = await httpClient.SendAsync(request);

        var responseString = await responseObject.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<BaseResponse<List<string>>>(responseString);

        return response?.Resource;
    }
}