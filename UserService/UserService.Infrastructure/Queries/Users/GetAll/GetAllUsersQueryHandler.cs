using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetAll;

public class GetAllUsersQueryHandler(UserServiceDbContext context) : IRequestHandler<GetAllUsersQuery, BaseResponse<List<UserDetails>>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<List<UserDetails>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

            var result =
                BaseResponse<List<UserDetails>>.Ok(users.Select(GenericMapper<User, UserDetails>.Map).ToList());

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<List<UserDetails>>.BadRequest([exception.Message]);
            return result;
        }
    }
}