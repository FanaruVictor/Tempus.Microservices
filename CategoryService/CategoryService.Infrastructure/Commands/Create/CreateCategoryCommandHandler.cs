using CategoryService.Core.Entities;
using CategoryService.Core.Models.Category;
using CategoryService.Data.Context;
using CategoryService.Infrastructure.Commons;
using MediatR;

namespace CategoryService.Infrastructure.Commands.UserCategory.Create;

public class CreateCategoryCommandHandler(CategoryServiceDbContext context) : IRequestHandler<CreateCategoryCommand, BaseResponse<BaseCategory>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<BaseCategory>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            Category entity;

            if (request.GroupId.HasValue)
            {
                var groupCategory = new GroupCategory
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Color = request.Color,
                    GroupId = request.GroupId.Value
                };

                await _context.Categories.AddAsync(groupCategory, cancellationToken);

                entity = groupCategory;
            }
            else
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Color = request.Color,
                    UserId = request.UserId
                };

                await _context.Categories.AddAsync(category, cancellationToken);

                entity = category;
            }

            BaseResponse<BaseCategory> result;


            await _context.SaveChangesAsync(cancellationToken);

            var baseCategory = GenericMapper<Category, BaseCategory>.Map(entity);
            result =
                BaseResponse<BaseCategory>.Ok(baseCategory);

            return result;
        }
        catch (Exception exception)
        {
            return BaseResponse<BaseCategory>.BadRequest([exception.Message]);
        }
    }
}