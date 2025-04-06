using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;

namespace UserService.Infrastructure.Queries.Users.GetUserPhotos
{
    public class GetUserPhotosQueryHandler(UserServiceDbContext context) : IRequestHandler<GetUserPhotosQuery, BaseResponse<List<string>>>
    {
        private readonly UserServiceDbContext context = context;

        public async Task<BaseResponse<List<string>>> Handle(GetUserPhotosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var photoUrls = await this.context.Photos
                    .AsNoTracking()
                    .Where(x => request.UserIds.Contains(x.UserId))
                    .Select(x => x.Url)
                    .ToListAsync();

                var result = BaseResponse<List<string>>.Ok(photoUrls);

                return result;
            }
            catch (Exception exception)
            {
                return BaseResponse<List<string>>.BadRequest([exception.Message]);
            }
        }
    }
}
