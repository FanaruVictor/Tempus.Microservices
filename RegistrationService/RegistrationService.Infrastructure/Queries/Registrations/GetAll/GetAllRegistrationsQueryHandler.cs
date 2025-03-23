using MediatR;
using RegistrationService.Core.Commons;
using RegistrationService.Core.Entities;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure.Commons;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetAll;

public class
    GetAllRegistrationsQueryHandler : IRequestHandler<GetAllRegistrationsQuery,
        BaseResponse<List<RegistrationDetails>>>
{
    private readonly RegistrationServiceDbContext _context;

    public GetAllRegistrationsQueryHandler(RegistrationServiceDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<List<RegistrationDetails>>> Handle(GetAllRegistrationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<Registration> registrations = null;

            if (request.GroupId.HasValue)
            {
                //get all registrations from group with groupId
                //get all categories with groupId
                //based on categories get the registration
            }
            else
            {
                //get all registration for user based on userId
                //get all categories for user
                //based on categories get registrations
            }


            var registrationsOverview = registrations
                .Select(x =>
                {
                    //should get the category based on registration.CategoryId and get the color
                    var categoryColor = "";

                    var currentRegistration = GenericMapper<Registration, RegistrationDetails>.Map(x);
                    currentRegistration.CategoryColor = categoryColor;

                    return currentRegistration;
                })
                .ToList();

            var response = BaseResponse<List<RegistrationDetails>>.Ok(registrationsOverview);
            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<List<RegistrationDetails>>.BadRequest(new List<string> { exception.Message });
            return response;
        }
    }
}