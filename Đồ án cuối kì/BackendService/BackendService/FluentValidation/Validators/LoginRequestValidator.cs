using BackendService.Core.DTOs.User.Requests;
using FluentValidation;

namespace BackendService.FluentValidation.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is Empty")
                .EmailAddress().WithMessage("Email error");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is Empty");

        }
    }
}
