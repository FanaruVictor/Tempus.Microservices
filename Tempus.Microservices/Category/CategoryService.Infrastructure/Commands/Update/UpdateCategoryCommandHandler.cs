using CategoryService.Core.Entities;
using CategoryService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Commands.UserCategory.Update;

public class UpdateCategoryCommandHandler(CategoryServiceDbContext context)
    : IRequestHandler<UpdateCategoryCommand, BaseResponse<BaseCategory>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<BaseCategory>> Handle(UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return BaseResponse<BaseCategory>.NotFound(
                    $"Category with Id: {request.Id} not found.");
            }

            if ((request.GroupId.HasValue && request.GroupId.Value != entity.OwnerId &&
                request.GroupId.Value != Guid.Empty) || request.UserId != entity.OwnerId)
            {
                return BaseResponse<BaseCategory>.Forbbiden();
            }

            entity = new Category
            {
                Id = entity.Id,
                Name = request.Name,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = DateTime.UtcNow,
                Color = request.Color,
                OwnerId = entity.OwnerId
            };

            _context.Categories.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var baseCategory = GenericMapper<Category, BaseCategory>.Map(entity);
            var result = BaseResponse<BaseCategory>.Ok(baseCategory);

            return result;
        }
        catch (Exception exception)
        {
            var result = BaseResponse<BaseCategory>.BadRequest([exception.Message]);
            return result;
        }
    }
}