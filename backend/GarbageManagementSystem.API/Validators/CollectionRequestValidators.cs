using FluentValidation;
using GarbageManagementSystem.API.DTOs.CollectionRequests;

namespace GarbageManagementSystem.API.Validators;

public class CreateCollectionRequestDtoValidator : AbstractValidator<CreateCollectionRequestDto>
{
    public CreateCollectionRequestDtoValidator()
    {
        RuleFor(x => x.WasteCategoryId).GreaterThan(0).WithMessage("Please select a waste category.");
        RuleFor(x => x.PickupDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Pickup date cannot be in the past.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateCollectionRequestStatusDtoValidator : AbstractValidator<UpdateCollectionRequestStatusDto>
{
    public UpdateCollectionRequestStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
