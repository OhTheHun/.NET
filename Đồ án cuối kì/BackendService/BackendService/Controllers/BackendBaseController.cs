using BackendService.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackendService.Controllers
{
    public class BackendBaseController(IOptions<ConfigOptions> options): ControllerBase
    {
        private readonly IOptions<ConfigOptions> _options = options;

        public string Username
        {
            get
            {
                return User?.Identity?.Name ?? "system";
            }
        }
        public string Actor
        {
            get
            {
                return User?.Identity?.Name ?? "anomynous";
            }
        }
    }
}
