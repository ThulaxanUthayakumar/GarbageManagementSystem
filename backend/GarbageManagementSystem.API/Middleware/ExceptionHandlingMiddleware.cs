using System.Text.Json;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;

namespace GarbageManagementSystem.API.Middleware;

/// <summary>
/// Catches every unhandled exception in the pipeline and turns it into a
/// consistent ApiResponse&lt;object&gt; JSON body with an appropriate status code,
/// instead of letting a raw 500 stack trace leak to the client.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "You are not authorized to perform this action."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailResponse(
            message,
            statusCode == StatusCodes.Status500InternalServerError ? null : new List<string> { exception.Message });

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
