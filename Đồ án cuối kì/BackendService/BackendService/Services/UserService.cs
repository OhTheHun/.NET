using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Data.Interface;
using BackendService.Mapping;
using BackendService.Model;
using BackendService.Services.Interface;

namespace BackendService.Services
{
    public class UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService): IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;

        public async Task<UserResponseDto[]> GetListUserAsync(string? keyword, IReadOnlyList<string>? roles, CancellationToken cancellationToken)
        {
            var listUser = await _userRepository.GetListUserAsync(keyword, roles, cancellationToken);
            return listUser.Select(user => UserToUserReponseDto.Tranform(user)).ToArray();
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            var mappedUser = UserToUserReponseDto.Tranform(user);
            return mappedUser;

        }

        public async Task<RegisterUserResponseDto> RegisterUserAsync(RegisterUserRequestDto registerUserResponeDto, string actor, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(registerUserResponeDto.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists");
            }
            var mappedUser = RegisterRequestDtoToUser.Transform(registerUserResponeDto, actor);
            mappedUser.Password = _passwordHasherService.Hash(registerUserResponeDto.Password);
            var user = await _userRepository.RegisterUserAsync(mappedUser, cancellationToken);
            var responseUser = UserToRegisterResponseDto.Transform(user);
            return responseUser;

        }
    }
}
