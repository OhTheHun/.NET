using BackendService.Common;
using BackendService.Configuration;
using BackendService.Constants;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;
using BackendService.Services.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace BackendService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(IOptions<ConfigOptions> options, IValidator<RegisterUserRequestDto> registerRequestValidator, IUserService userService) : BackendBaseController(options)
    {
        private readonly ConfigOptions _configOptions = options.Value;
        private readonly IValidator<RegisterUserRequestDto> _registerRequestValidator = registerRequestValidator;
        private readonly IUserService _userService = userService;

        [HttpPost("register")]

        public async Task<ActionResult<RegisterUserResponseDto?>> RegisterUserAsync([FromBody] RegisterUserRequestDto registerRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var actor = Actor;
                var validationResult = await _registerRequestValidator.ValidateAsync(registerRequestDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ValidationResultToCustomValidationResult.Map(validationResult.Errors));
                }
                var registerResponse = await _userService.RegisterUserAsync(registerRequestDto, actor, cancellationToken);
                return Ok(registerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{ConstantValue.UserRole.Admin}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var userResponse = await _userService.GetUserByIdAsync(id, cancellationToken);
                if (userResponse == null)
                {
                    return NotFound(new { error = "User not found" });
                }
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }

        }
        [HttpGet("list")]
        [Authorize(Roles = $"{ConstantValue.UserRole.Admin}")]
        public async Task<ActionResult<UserResponseDto[]>> GetAllUsersAsync([FromQuery] string? keyword, [FromQuery] string? roles, CancellationToken cancellationToken)
        {
            try
            {
                var roleList = roles?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .ToList();

                var users = await _userService.GetListUserAsync(keyword, roleList, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }

        }
    }
}
