using BackendService.Core.DTOs.User.Requests;
using FluentValidation;

namespace BackendService.FluentValidation.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequestDto>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email cannot be empty")
                    .EmailAddress().WithMessage("Wrong email template");
            
            RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}
