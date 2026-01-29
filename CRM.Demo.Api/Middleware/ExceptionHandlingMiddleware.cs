using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Api.Middleware;

/// <summary>
/// Middleware do obsługi wyjątków globalnie.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case DomainException domainException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = domainException.Message });
                break;
            case FluentValidation.ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    error = "Validation failed",
                    errors = validationException.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
                break;
            default:
                result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request" });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
