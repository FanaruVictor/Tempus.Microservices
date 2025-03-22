using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Commands.Users.Create
{
    public class CreateUserCommandHandler(UserServiceDbContext context) : IRequestHandler<CreateUserCommand, BaseResponse<UserDetails>>
    {
        private readonly UserServiceDbContext _context = context;

        public async Task<BaseResponse<UserDetails>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                cancellationToken.ThrowIfCancellationRequested();

                if (await IsEmailAlreadyRegistered(request.Email))
                {
                    return BaseResponse<UserDetails>.BadRequest(new()
                    {
                        "Email already in use"
                    });
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Username = request.UserName,
                    Password = request.Password
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var userDetails = GenericMapper<User, UserDetails>.Map(user);

                var result = BaseResponse<UserDetails>.Ok(userDetails);

                return result;

            }
            catch (Exception exception)
            {
                return BaseResponse<UserDetails>.BadRequest([exception.Message]);
            }
        }
        private async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
    }
}
