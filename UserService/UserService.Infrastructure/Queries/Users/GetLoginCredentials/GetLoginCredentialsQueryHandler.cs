using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetUserByEmail
{
    public class GetUserByEmailQueryHandler(UserServiceDbContext context) : IRequestHandler<GetLoginCredentialsHandler, BaseResponse<LoginCredentials>>
    {
        private readonly UserServiceDbContext _context = context;

        public async Task<BaseResponse<LoginCredentials>> Handle(GetLoginCredentialsHandler request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var user = await _context.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

                if (user == null)
                {
                    return BaseResponse<LoginCredentials>.NotFound("User not found.");
                }

                var userDetails = GenericMapper<User, LoginCredentials>.Map(user);


                var result = BaseResponse<LoginCredentials>.Ok(userDetails);

                return result;
            }
            catch (Exception exception)
            {
                var result = BaseResponse<LoginCredentials>.BadRequest([exception.Message]);

                return result;
            }
        }
    }
}
