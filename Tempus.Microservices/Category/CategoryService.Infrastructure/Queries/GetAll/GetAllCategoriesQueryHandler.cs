using CategoryService.Core.Entities;
using CategoryService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Queries.Categories.GetAll;

public class GetAllCategoriesQueryHandler(CategoryServiceDbContext context)
    : IRequestHandler<GetAllCategoriesQuery, BaseResponse<List<BaseCategory>>>
{
    private readonly CategoryServiceDbContext _context = context;

    public async Task<BaseResponse<List<BaseCategory>>> Handle(GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(x => x.OwnerId == (request.GroupId.HasValue && request.GroupId.Value != Guid.Empty
                    ? request.GroupId.Value
                    : request.UserId))
                .Select(x => x)
                .ToListAsync(cancellationToken);

            var response =
                BaseResponse<List<BaseCategory>>.Ok(categories
                    .Select(GenericMapper<Category, BaseCategory>.Map).ToList());

            return response;
        }
        catch(Exception exception)
        {
            var response = BaseResponse<List<BaseCategory>>.BadRequest([exception.Message]);
            return response;
        }
    }
}