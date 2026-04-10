using BackendService.Configuration;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Data.Interface;
using BackendService.Model;
using BackendService.Services.Interface;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace BackendService.Services
{
    public class AuthService(IUserRepository userRepository, IOptions<ConfigOptions> configOptions, IPasswordHasherService passwordHasherService): IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ConfigOptions _configOptions = configOptions.Value;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                // tạo danh sách claim dữ liệu (iat: thời điểm token tạo, thời gian hiện tạim 
                new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                // tạo token riêng cho role
                new(ClaimTypes.Role, user.Role),
            };
            // tạo khóa ký token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configOptions.JwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // tạo token
            var token = new JwtSecurityToken(
                issuer: _configOptions.JwtConfig.Issuer,
                audience: _configOptions.JwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMilliseconds(Convert.ToDouble(_configOptions.JwtConfig.TokenValidityMiliSeconds)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken = default)
        {
            var userInDb = await _userRepository.GetUserByUsernameAsync(loginRequestDto.Username, cancellationToken);
            if (userInDb == null)
            {
                return null;
            }

            var isVerifiedPassword = _passwordHasherService.VerifyPassword(loginRequestDto.Password, userInDb.Password);
            if (!isVerifiedPassword)
            {
                return null;
            }

            return new LoginResponseDto
            {
                AccessToken = GenerateToken(userInDb),
                ExpireIn = _configOptions.JwtConfig.TokenValidityMiliSeconds,
                UserId = userInDb.Id,
                Role = userInDb.Role
            };
        }
    }
}
