using MediatR;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Data.Context;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetAllEmails;

public class GetAllEmailsQueryHandler(UserServiceDbContext context) : IRequestHandler<GetAllEmailsQuery, BaseResponse<List<UserEmail>>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<List<UserEmail>>> Handle(GetAllEmailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userEmails = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id != request.UserId)
                .Include(x => x.Photo)
                .Select(x => new UserEmail
                {
                    Email = x.Email,
                    Id = x.Id,
                    PhotoUrl = x.Photo.Url
                }
                )
                .ToListAsync(cancellationToken: cancellationToken);

            return BaseResponse<List<UserEmail>>.Ok(userEmails);
        }
        catch (Exception e)
        {
            return BaseResponse<List<UserEmail>>.BadRequest(
            [
                e.Message
            ]);
        }
    }
}