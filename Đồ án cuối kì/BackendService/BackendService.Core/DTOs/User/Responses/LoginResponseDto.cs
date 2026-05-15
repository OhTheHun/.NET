namespace BackendService.Core.DTOs.User.Responses
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpireIn { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? FullName { get; set; }

    }
}
