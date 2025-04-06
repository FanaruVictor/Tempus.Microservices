using CategoryService.Core.Entities;
using CategoryService.Data.Context;
using MediatR;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Commands.UserCategory.Create;

public class CreateCategoryCommandHandler(CategoryServiceDbContext context)
    : IRequestHandler<CreateCategoryCommand, BaseResponse<BaseCategory>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<BaseCategory>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                Color = request.Color,
                OwnerId = request.GroupId.HasValue && request.GroupId.Value != Guid.Empty
                    ? request.GroupId.Value
                    : request.UserId
            };

            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            BaseResponse<BaseCategory> result;

            var baseCategory = GenericMapper<Category, BaseCategory>.Map(category);
            result =
                BaseResponse<BaseCategory>.Ok(baseCategory);

            return result;
        }
        catch(Exception exception)
        {
            return BaseResponse<BaseCategory>.BadRequest([exception.Message]);
        }
    }
}