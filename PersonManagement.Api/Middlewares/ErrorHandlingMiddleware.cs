namespace PersonDirectory.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "დაუმუშავებელი შეცდომა მოხდა: {Message} - Path: {Path} - Method: {Method}",
                ex.Message, context.Request.Path, context.Request.Method);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Get localizer from DI container
        using var scope = context.RequestServices.CreateScope();
        var localizer = scope.ServiceProvider.GetService<IStringLocalizer<ErrorHandlingMiddleware>>();

        var (statusCode, message) = GetErrorResponse(exception, localizer);
        response.StatusCode = statusCode;

        var errorResponse = new
        {
            success = false,
            message = message,
            details = GetErrorDetails(exception, context)
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static (int statusCode, string message) GetErrorResponse(Exception exception, IStringLocalizer<ErrorHandlingMiddleware>? localizer)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException =>
                (400, localizer?[ErrorMessages.ValidationFailed] ?? "Validation failed"),
            UnauthorizedAccessException =>
                (401, localizer?[ErrorMessages.UnauthorizedAccess] ?? "Unauthorized access"),
            NotImplementedException =>
                (501, "Feature not implemented"),
            TimeoutException =>
                (408, "Request timeout"),
            _ => (500, localizer?[ErrorMessages.InternalServerError] ?? "An internal server error occurred")
        };
    }

    private static object? GetErrorDetails(Exception exception, HttpContext context)
    {
        // Only include detailed error information in development
        var environment = context.RequestServices.GetService<IWebHostEnvironment>();

        if (environment?.IsDevelopment() == true)
        {
            return new
            {
                type = exception.GetType().Name,
                stackTrace = exception.StackTrace,
                innerException = exception.InnerException?.Message
            };
        }

        return null;
    }
}