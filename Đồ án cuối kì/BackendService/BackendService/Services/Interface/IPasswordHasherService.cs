namespace BackendService.Services.Interface
{
    public interface IPasswordHasherService
    {
        public string Hash(string password);
        public bool VerifyPassword(string password, string passwordHash);
    }
}
