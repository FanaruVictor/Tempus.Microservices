using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Tempus.Shared.Commons;
using UserService.Data.Context;

namespace UserService.Infrastructure.Commands.Users.Delete;

public class DeleteUserCommandHandler(
    //ICloudinaryService cloudinaryService,
    UserServiceDbContext context,
    IConnection messageConnection,
    IConfiguration configuration
    ) : IRequestHandler<DeleteUserCommand, BaseResponse<Guid>>
{
    private readonly UserServiceDbContext _context = context;
    private readonly IConnection messageConnection = messageConnection;
    private readonly IConfiguration configuration = configuration;
    //private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

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
                //await _cloudinaryService.DestroyUsingUserId(deletedUserId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            SendDeleteUserMessage(deletedUserId);

            result = BaseResponse<Guid>.Ok(deletedUserId);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<Guid>.BadRequest([exception.Message]);
            return result;
        }
    }

    private void SendDeleteUserMessage(Guid userId)
    {
        using var channel = messageConnection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "deleteUserExchange",
            ExchangeType.Fanout
        );

        channel.BasicPublish(
            exchange: "deleteUserExchange",
            routingKey: "",
            basicProperties: null,
            body: System.Text.Encoding.UTF8.GetBytes(userId.ToString())
        );
    }
}