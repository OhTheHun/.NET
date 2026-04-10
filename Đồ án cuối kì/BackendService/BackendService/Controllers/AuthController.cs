using BackendService.Common;
using BackendService.Configuration;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Services.Interface;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackendService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IOptions<ConfigOptions> options, IValidator<LoginRequestDto> loginRequestValidator, IAuthService authService): BackendBaseController(options)
    {
        private readonly ConfigOptions _options = options.Value;
        private readonly IValidator<LoginRequestDto> _loginRequestValidator = loginRequestValidator;
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto?>> LoginAsync([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _loginRequestValidator.ValidateAsync(loginRequestDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return BadRequest(ValidationResultToCustomValidationResult.Map(validationResult.Errors));
                }
                var loginResponse = await _authService.LoginAsync(loginRequestDto, cancellationToken);
                if (loginResponse == null)
                {
                    return Unauthorized(new { message = "UserName or Password is incorrect" });
                }

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
