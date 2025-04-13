using CategoryService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Tempus.Shared.Commons;

namespace CategoryService.Infrastructure.Commands.Registrations.DeleteAllForOwner
{
    public class DeleteAllCategoriesForOwnerCommandHandler(
        CategoryServiceDbContext context,
        IConnection messageConnection
        ) : IRequestHandler<DeleteAllCategoriesForOwnerCommand, BaseResponse<bool>>
    {
        private readonly CategoryServiceDbContext context = context;

        public async Task<BaseResponse<bool>> Handle(DeleteAllCategoriesForOwnerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guid ownerId = Guid.Empty;

                if (!Guid.TryParse(request.OwnerId, out ownerId))
                {
                    return BaseResponse<bool>.NotFound("Category not found");
                }

                var categories = await context.Categories
                    .AsNoTracking()
                    .Where(x => x.OwnerId == ownerId)
                    .ToListAsync(cancellationToken);

                context.RemoveRange(categories);

                await context.SaveChangesAsync(cancellationToken);

                foreach (var category in categories)
                {
                    SendDeleteCategoryMessage(category.Id);
                }

                return BaseResponse<bool>.Ok(true);
            }
            catch (Exception exception)
            {
                return BaseResponse<bool>.BadRequest(new List<string> { exception.Message });
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
}
