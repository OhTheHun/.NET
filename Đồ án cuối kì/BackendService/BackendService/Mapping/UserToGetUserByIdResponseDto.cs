using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class UserToGetUserByIdResponseDto
    {
        public static GetUserByIdResponseDto Transform(User user)
        {
            return new GetUserByIdResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
        }
    }
}
