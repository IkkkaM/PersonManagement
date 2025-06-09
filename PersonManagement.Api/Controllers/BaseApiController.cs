namespace PersonDirectory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IStringLocalizer _localizer;

    protected BaseApiController(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Data = result.Data,
                Message = null
            });
        }

        if (result.ValidationErrors.Any())
        {
            return BadRequest(new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = _localizer[ErrorMessages.ValidationFailed],
                Errors = result.ValidationErrors
            });
        }

        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = result.ErrorMessage
        });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = null
            });
        }

        if (result.ValidationErrors.Any())
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = _localizer[ErrorMessages.ValidationFailed],
                Errors = result.ValidationErrors
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = result.ErrorMessage
        });
    }

    protected IActionResult NotFound(string message)
    {
        return NotFound(new ApiResponse
        {
            Success = false,
            Message = message
        });
    }

}