using BackendService.Constants;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class RegisterRequestDtoToUser
    {
        public static User Transform(RegisterUserRequestDto dto, string actor)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email ?? string.Empty,
                Password = dto.Password,
                Role = ConstantValue.UserRole.Customer,
                CreatedBy = actor,
                UpdatedBy = actor,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow,
                DeleteFlag = false,
                IsActive = true,
            };
        }
    }
}
