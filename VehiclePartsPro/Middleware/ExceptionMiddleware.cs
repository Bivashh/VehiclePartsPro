using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace VehiclePartsPro.Middleware;


public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass request to the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            //  LogError for handled exceptions that shouldn't crash the app
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        // Map known exception types to HTTP status codes
        context.Response.StatusCode = ex switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = context.Response.StatusCode == 500
                ? "An unexpected error occurred."   // don't expose internals
                : ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}