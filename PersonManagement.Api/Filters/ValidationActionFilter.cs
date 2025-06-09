using Microsoft.AspNetCore.Mvc;
namespace PersonDirectory.Api.Filters;

public class ValidationActionFilter : IActionFilter
{
    private readonly IStringLocalizer<ValidationActionFilter> _localizer;

    public ValidationActionFilter(IStringLocalizer<ValidationActionFilter> localizer)
    {
        _localizer = localizer;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new
            {
                success = false,
                message = _localizer[ErrorMessages.ValidationFailed],
                errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}