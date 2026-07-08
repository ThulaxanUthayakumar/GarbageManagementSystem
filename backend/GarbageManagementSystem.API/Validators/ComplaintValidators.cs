using FluentValidation;
using GarbageManagementSystem.API.DTOs.Complaints;

namespace GarbageManagementSystem.API.Validators;

public class CreateComplaintDtoValidator : AbstractValidator<CreateComplaintDto>
{
    public CreateComplaintDtoValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

public class UpdateComplaintStatusDtoValidator : AbstractValidator<UpdateComplaintStatusDto>
{
    public UpdateComplaintStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.AdminRemarks).MaximumLength(500);
    }
}
