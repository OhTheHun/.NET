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
    public class AuthController(IOptions<ConfigOptions> options, IValidator<LoginRequestDto> loginRequestValidator, IAuthService authService, IValidator<ForgotPasswordRequestDto> forgotPasswordValidator, IValidator<ResetPasswordRequestDto> resetPasswordValidator): BackendBaseController(options)
    {
        private readonly ConfigOptions _options = options.Value;
        private readonly IValidator<LoginRequestDto> _loginRequestValidator = loginRequestValidator;
        private readonly IAuthService _authService = authService;
        private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator = forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator = resetPasswordValidator;

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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _forgotPasswordValidator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ValidationResultToCustomValidationResult.Map(validationResult.Errors));
                }

                var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
                if (!result)
                {
                    // For security reasons, don't reveal if email exists or not
                    return Ok(new { message = "If the email is valid, a password reset link has been sent." });
                }

                return Ok(new { message = "If the email is valid, a password reset link has been sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _resetPasswordValidator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ValidationResultToCustomValidationResult.Map(validationResult.Errors));
                }

                var result = await _authService.ResetPasswordAsync(request, cancellationToken);
                if (!result)
                {
                    return BadRequest(new { message = "Invalid email, reset code, or the code has expired." });
                }

                return Ok(new { message = "Password has been successfully reset." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
