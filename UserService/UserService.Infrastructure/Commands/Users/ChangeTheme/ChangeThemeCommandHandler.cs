using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Commands.Users.ChangeTheme;

public class ChangeThemeCommandHandler(UserServiceDbContext context) : IRequestHandler<ChangeThemeCommand, BaseResponse<UserDetails>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<UserDetails>> Handle(ChangeThemeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.Photo)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);

            BaseResponse<UserDetails> result;

            if (user == null)
            {
                result = BaseResponse<UserDetails>.NotFound("User not found");

                return result;
            }

            user.IsDarkTheme = request.IsDarkTheme;

            _context.Users.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var profilePhoto = user.Photo;

            var userDetails = GenericMapper<User, UserDetails>.Map(user);

            if (profilePhoto != null)
            {
                userDetails.Photo = GenericMapper<Photo, PhotoDetails>.Map(profilePhoto);
            }

            result = BaseResponse<UserDetails>.Ok(userDetails);

            return result;
        }
        catch (Exception exception)
        {
            return BaseResponse<UserDetails>.BadRequest(
            [
                exception.Message
            ]);
        }
    }
}