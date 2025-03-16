using CategoryService.Core.Entities;
using CategoryService.Core.Models.Category;
using CategoryService.Data.Context;
using CategoryService.Infrastructure.Commons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Queries.Categories.GetAll;

public class GetAllCategoriesQueryHandler(CategoryServiceDbContext context) : IRequestHandler<GetAllCategoriesQuery, BaseResponse<List<BaseCategory>>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<List<BaseCategory>>> Handle(GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<Category> categories;

            if (request.GroupId.HasValue)
            {
                categories = await _context.GroupCategories
                    .AsNoTracking()
                    .Where(x => x.GroupId == request.GroupId)
                    .Select(x => (Category)x)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                categories = await _context.Categories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .ToListAsync(cancellationToken);
            }

            var response =
                BaseResponse<List<BaseCategory>>.Ok(categories
                    .Select(GenericMapper<Category, BaseCategory>.Map).ToList());

            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<List<BaseCategory>>.BadRequest([exception.Message]);
            return response;
        }
    }
}