using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;

namespace UserService.Infrastructure.Queries.Users.GetTheme;

public class GetThemeQueryHandle(UserServiceDbContext context) : IRequestHandler<GetThemeQuery, BaseResponse<bool>>
{
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<bool>> Handle(GetThemeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var theme = (await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken))
                ?.IsDarkTheme;

            BaseResponse<bool> result;

            if (!theme.HasValue)
            {
                result = BaseResponse<bool>.NotFound("Theme for this user not found");
                return result;
            }

            result = BaseResponse<bool>.Ok(theme.Value);

            return result;
        }
        catch (Exception exception)
        {
            return BaseResponse<bool>.BadRequest([exception.Message]);
        }
    }
}