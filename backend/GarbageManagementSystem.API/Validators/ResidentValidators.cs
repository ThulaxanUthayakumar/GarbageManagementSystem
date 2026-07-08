using FluentValidation;
using GarbageManagementSystem.API.DTOs.Residents;

namespace GarbageManagementSystem.API.Validators;

public class CreateResidentDtoValidator : AbstractValidator<CreateResidentDto>
{
    public CreateResidentDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
        RuleFor(x => x.Zone).NotEmpty();
    }
}

public class UpdateResidentDtoValidator : AbstractValidator<UpdateResidentDto>
{
    public UpdateResidentDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
        RuleFor(x => x.Zone).NotEmpty();
    }
}
