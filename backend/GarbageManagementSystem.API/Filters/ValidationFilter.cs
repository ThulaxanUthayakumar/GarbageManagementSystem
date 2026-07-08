using FluentValidation;
using GarbageManagementSystem.API.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GarbageManagementSystem.API.Filters;

/// <summary>
/// Global action filter that automatically runs any registered FluentValidation
/// validator against incoming action arguments before the controller action runs.
/// This keeps controllers free of repetitive "if (!ModelState.IsValid) ..." checks.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(value.GetType());

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(value);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                context.Result = new BadRequestObjectResult(ApiResponse<object>.FailResponse("Validation failed.", errors));
                return;
            }
        }

        await next();
    }
}
