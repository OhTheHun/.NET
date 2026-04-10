using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class UserToUserReponseDto
    {
        public static UserResponseDto Tranform(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
