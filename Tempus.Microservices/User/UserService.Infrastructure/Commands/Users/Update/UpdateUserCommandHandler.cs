using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.IServices;
using UserService.Infrastructure.Models;
using StatusCodes = Tempus.Shared.Commons.StatusCodes;

namespace UserService.Infrastructure.Commands.Users.Update;

public class UpdateUserCommandHandler(
    ICloudinaryService cloudinaryService,
    UserServiceDbContext context) : IRequestHandler<UpdateUserCommand, BaseResponse<UserDetails>>
{
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
    private readonly UserServiceDbContext _context = context;

    public async Task<BaseResponse<UserDetails>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.Photo)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                return BaseResponse<UserDetails>.NotFound($"User with id {request.UserId} not .");
            }

            var updateResult = await UpdateUser(request, user);

            BaseResponse<UserDetails> result;
            if (updateResult.StatusCode != StatusCodes.Ok)
            {
                result = new BaseResponse<UserDetails>
                {
                    Errors = updateResult.Errors,
                    StatusCode = updateResult.StatusCode
                };
                return result;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var userDetails = GenericMapper<User, UserDetails>.Map(updateResult.Resource);
            userDetails.Photo = GenericMapper<Photo, PhotoDetails>.Map(updateResult.Resource.Photo);

            result = BaseResponse<UserDetails>.Ok(userDetails);

            return result;
        }
        catch (Exception exception)
        {
            return BaseResponse<UserDetails>.BadRequest([exception.Message]);
        }
    }

    private async Task<BaseResponse<User>> UpdateUser(UpdateUserCommand request, User user)
    {
        user = new User
        {
            Id = user.Id,
            Username = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsDarkTheme = user.IsDarkTheme,
            Photo = user.Photo
        };

        if (request.IsPhotoChanged)
        {
            var updatePhotoResult = await UpdatePhoto(request.NewPhoto, user);

            if (updatePhotoResult.StatusCode != StatusCodes.Ok)
            {
                return new BaseResponse<User>
                {
                    Errors = updatePhotoResult.Errors,
                    StatusCode = updatePhotoResult.StatusCode,
                };
            }

            user.Photo = updatePhotoResult.Resource ?? null;
        }

        _context.Users.Update(user);
        return BaseResponse<User>.Ok(user);
    }

    private async Task<BaseResponse<Photo>> UpdatePhoto(IFormFile? photo, User user)
    {
        if (photo == null)
        {
            if (user.Photo != null)
            {
                await _cloudinaryService.DestroyUsingUserId(user.Id);
                _context.Photos
                    .Remove(user.Photo);
            }

            return BaseResponse<Photo>.Ok();
        }

        ImageUploadResult uploadResult;

        Photo userPhoto;

        if (user.Photo != null)
        {
            await _cloudinaryService.DestroyUsingUserId(user.Id);

            uploadResult = await _cloudinaryService.Upload(photo);

            userPhoto = new Photo
            {
                Id = user.Photo.Id,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                UserId = user.Id
            };

            _context.Photos.Update(userPhoto);
        }
        else
        {
            uploadResult = await _cloudinaryService.Upload(photo);
            userPhoto = new Photo
            {
                Id = Guid.NewGuid(),
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                UserId = user.Id
            };
            await _context.AddAsync(userPhoto);
        }

        await _context.SaveChangesAsync();

        return BaseResponse<Photo>.Ok(userPhoto);
    }
}