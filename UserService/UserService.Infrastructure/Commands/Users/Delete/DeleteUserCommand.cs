using UserService.Infrastructure.Commons;

namespace UserService.Infrastructure.Commands.Users.Delete;

public class DeleteUserCommand : BaseRequest<BaseResponse<Guid>> { }