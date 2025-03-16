using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commands.Users.Update;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.IServices;
using UserService.Infrastructure.Models;
using StatusCodes = UserService.Infrastructure.Commons.StatusCodes;

namespace Tempus.Infrastructure.Commands.Users.Update;

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
                .Include(x => x.UserPhoto)
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
            userDetails.Photo = GenericMapper<UserPhoto, PhotoDetails>.Map(updateResult.Resource.UserPhoto);

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
            ExternalId = user.ExternalId,
            UserPhoto = user.UserPhoto
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

            user.UserPhoto = updatePhotoResult.Resource ?? null;
        }

        _context.Users.Update(user);
        return BaseResponse<User>.Ok(user);
    }

    private async Task<BaseResponse<UserPhoto>> UpdatePhoto(IFormFile? photo, User user)
    {
        if (photo == null)
        {
            if (user.UserPhoto != null)
            {
                await _cloudinaryService.DestroyUsingUserId(user.Id);
                _context.UserPhotos
                    .Remove(user.UserPhoto);
            }

            return BaseResponse<UserPhoto>.Ok();
        }

        ImageUploadResult uploadResult;

        UserPhoto userPhoto;

        if (user.UserPhoto != null)
        {
            await _cloudinaryService.DestroyUsingUserId(user.Id);

            uploadResult = await _cloudinaryService.Upload(photo);

            userPhoto = new UserPhoto
            {
                Id = user.UserPhoto.Id,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                UserId = user.Id
            };

            _context.UserPhotos.Update(userPhoto);
        }
        else
        {
            uploadResult = await _cloudinaryService.Upload(photo);
            userPhoto = new UserPhoto
            {
                Id = Guid.NewGuid(),
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                UserId = user.Id
            };
            await _context.AddAsync(userPhoto);
        }

        await _context.SaveChangesAsync();

        return BaseResponse<UserPhoto>.Ok(userPhoto);
    }
}