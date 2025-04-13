using CategoryService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Tempus.Shared.Commons;

namespace CategoryService.Infrastructure.Commands.UserCategory.Delete;

public class DeleteCategoryCommandHandler(
    CategoryServiceDbContext context,
    IConnection messageConnection
    )
    : IRequestHandler<DeleteCategoryCommand, BaseResponse<Guid>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<Guid>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var deletedCategoryId = request.Id;

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == deletedCategoryId, cancellationToken);

            BaseResponse<Guid> result;

            if (category == null)
            {
                result = BaseResponse<Guid>.NotFound($"Category with Id: {request.Id} not found");
                return result;
            }

            if ((request.GroupId.HasValue && request.GroupId.Value != category.OwnerId &&
                request.GroupId.Value != Guid.Empty) || request.UserId != category.OwnerId)
            {
                return BaseResponse<Guid>.Forbbiden();
            }

            _context.Categories
                .Remove(category);

            await _context.SaveChangesAsync(cancellationToken);

            SendDeleteCategoryMessage(deletedCategoryId);

            result = BaseResponse<Guid>.Ok(deletedCategoryId);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<Guid>.BadRequest([exception.Message]);
            return result;
        }
    }
    private void SendDeleteCategoryMessage(Guid categoryId)
    {
        using var channel = messageConnection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "deleteCategoryExchange",
            ExchangeType.Fanout
        );

        channel.BasicPublish(
            exchange: "deleteCategoryExchange",
            routingKey: "",
            basicProperties: null,
            body: System.Text.Encoding.UTF8.GetBytes(categoryId.ToString())
        );
    }
}