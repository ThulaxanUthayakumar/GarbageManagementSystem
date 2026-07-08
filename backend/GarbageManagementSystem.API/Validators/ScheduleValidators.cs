using FluentValidation;
using GarbageManagementSystem.API.DTOs.Schedules;

namespace GarbageManagementSystem.API.Validators;

public class CreateScheduleDtoValidator : AbstractValidator<CreateScheduleDto>
{
    public CreateScheduleDtoValidator()
    {
        RuleFor(x => x.Zone).NotEmpty().WithMessage("Zone is required.");
        RuleFor(x => x.ScheduledDate).NotEmpty();
        RuleFor(x => x.ScheduledTime).NotEmpty().WithMessage("Scheduled time window is required, e.g. '08:00 AM - 11:00 AM'.");
    }
}

public class UpdateScheduleDtoValidator : AbstractValidator<UpdateScheduleDto>
{
    public UpdateScheduleDtoValidator()
    {
        RuleFor(x => x.Zone).NotEmpty().WithMessage("Zone is required.");
        RuleFor(x => x.ScheduledDate).NotEmpty();
        RuleFor(x => x.ScheduledTime).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}
