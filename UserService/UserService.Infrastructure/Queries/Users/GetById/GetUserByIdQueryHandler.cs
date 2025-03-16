using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetById;

public class GetUserByIdQueryHandler(UserServiceDbContext context) : IRequestHandler<GetUserByIdQuery, BaseResponse<UserDetails>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<UserDetails>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var id = request.Id;

            if (id == null)
            {
                id = request.UserId;
            }

            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.UserPhoto)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

            if (user == null)
            {
                return BaseResponse<UserDetails>.NotFound("User not found.");
            }

            var userDetails = GenericMapper<User, UserDetails>.Map(user);

            if (user.UserPhoto != null)
            {
                userDetails.Photo = GenericMapper<UserPhoto, PhotoDetails>.Map(user.UserPhoto);
            }

            var result = BaseResponse<UserDetails>.Ok(userDetails);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<UserDetails>.BadRequest([exception.Message]);

            return result;
        }
    }
}