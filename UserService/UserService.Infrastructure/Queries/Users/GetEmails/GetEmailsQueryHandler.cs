using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetEmails;

public class GetEmailsQueryHandler(UserServiceDbContext context) : IRequestHandler<GetEmailsQuery, BaseResponse<List<UserEmail>>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<List<UserEmail>>> Handle(GetEmailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userEmails = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id != request.UserId)
                .Include(x => x.UserPhoto)
                .Select(x => new UserEmail
                {
                    Email = x.Email,
                    Id = x.Id,
                    PhotoUrl = x.UserPhoto.Url
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