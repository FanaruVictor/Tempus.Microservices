using CategoryService.Core.Entities;
using CategoryService.Core.Models.Category;
using CategoryService.Data.Context;
using CategoryService.Infrastructure.Commons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Queries.Categories.GetById;

public class GetCategoryByIdQueryHandler(CategoryServiceDbContext context) : IRequestHandler<GetCategoryByIdQuery, BaseResponse<BaseCategory>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<BaseCategory>> Handle(GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return BaseResponse<BaseCategory>.NotFound("Category not found.");
            }

            if ((request.GroupId.HasValue && category.OwnerId != request.GroupId.Value && request.GroupId.Value != Guid.Empty) || category.OwnerId != request.UserId)
            {
                return BaseResponse<BaseCategory>.Forbbiden();
            }

            var baseCategory = GenericMapper<Category, BaseCategory>.Map(category);
            var response = BaseResponse<BaseCategory>.Ok(baseCategory);

            return response;
        }
        catch (Exception exception)
        {
            return BaseResponse<BaseCategory>.BadRequest([exception.Message]);
        }
    }
}