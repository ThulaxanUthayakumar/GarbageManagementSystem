using FluentValidation;
using GarbageManagementSystem.API.DTOs.Auth;

namespace GarbageManagementSystem.API.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name is required.").MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Enter a valid email address.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.State).NotEmpty().WithMessage("State is required.");
        RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip code is required.");
        RuleFor(x => x.Zone).NotEmpty().WithMessage("Collection zone is required.");
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Enter a valid email address.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage("New passwords do not match.");
    }
}
