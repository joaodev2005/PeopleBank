using System.Net;
using System.Text.Json;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (PeopleBankExceptions ex)
        {
            await HandlePeopleBankExceptionAsync(context, ex);
        }
        catch (System.Exception ex)
        {
            await HandleUnhandledExceptionAsync(context, ex);
        }
    }

    private static async Task HandlePeopleBankExceptionAsync(HttpContext context, PeopleBankExceptions exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)exception.GetStatusCode();

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = GetTitle(exception),
            status = context.Response.StatusCode,
            detail = string.Join("; ", exception.GetErrorMessages()),
            instance = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleUnhandledExceptionAsync(HttpContext context, System.Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "Internal Server Error",
            status = context.Response.StatusCode,
            detail = "An unexpected error occurred.",
            instance = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(PeopleBankExceptions exception) => exception switch
    {
        ErrorOnValidationException => "Validation Error",
        NotFoundException => "Resource Not Found",
        DomainException => "Business Rule Violation",
        _ => "Error"
    };
}