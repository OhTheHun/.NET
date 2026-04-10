using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class UserToRegisterResponseDto
    {
        public static RegisterUserResponseDto Transform(User user)
        {
            return new RegisterUserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
            };
        }
    }
}
