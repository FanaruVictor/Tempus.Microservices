using MediatR;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using UserService.Data.Context;
using UserService.Infrastructure.IServices;

namespace UserService.Infrastructure.Commands.Users.Delete;

public class DeleteUserCommandHandler(ICloudinaryService cloudinaryService, UserServiceDbContext context) : IRequestHandler<DeleteUserCommand, BaseResponse<Guid>>
{
    private readonly UserServiceDbContext _context = context;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<BaseResponse<Guid>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);

            BaseResponse<Guid> result;

            if (user == null)
            {
                result = BaseResponse<Guid>.NotFound($"User with Id: {request.UserId} not found");
                return result;
            }

            var deletedUserId = request.UserId;

            _context.Users
               .Remove(user);

            if (user.Photo != null)
            {
                await _cloudinaryService.DestroyUsingUserId(deletedUserId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            result = BaseResponse<Guid>.Ok(deletedUserId);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<Guid>.BadRequest([exception.Message]);
            return result;
        }
    }
}