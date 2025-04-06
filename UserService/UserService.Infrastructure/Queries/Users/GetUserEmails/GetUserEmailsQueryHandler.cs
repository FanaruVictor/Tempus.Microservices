using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetUserEmails
{
    public class GetUserEmailsQueryHandler(UserServiceDbContext context) : IRequestHandler<GetUserEmailsQuery, BaseResponse<List<UserEmail>>>
    {
        private readonly UserServiceDbContext context = context;

        public async Task<BaseResponse<List<UserEmail>>> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var userEmails = await this.context.Users
                       .AsNoTracking()
                       .Where(x => request.UserIds.Any(y => y.Equals(x.Id)))
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
            catch (Exception exception)
            {
                return BaseResponse<List<UserEmail>>.BadRequest([exception.Message]);
            }
        }
    }
}
