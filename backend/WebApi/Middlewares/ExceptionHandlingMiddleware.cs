using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using FluentValidation;
using Npgsql;

namespace AgendaPlus.WebApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex) when (IsDatabaseConnectivityError(ex))
        {
            logger.LogError(ex, "Database connectivity issue detected");
            await HandleDatabaseExceptionAsync(context,
                "Service temporarily unavailable. Please try again in a moment.");
        }
        catch (PostgresException ex)
        {
            logger.LogError(ex, "Postgres issue detected");
            await HandleDatabaseExceptionAsync(context,
                "An error occurred while saving data. Our team is already working to resolve the issue. Please try again later.");
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        logger.LogWarning(exception, "Validation error occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "One or more validation errors occurred.",
            status = 400,
            errors
        };

        var json = JsonSerializer.Serialize(response, _jsonSerializerOptions);

        await context.Response.WriteAsync(json);
    }

    private static Task HandleDatabaseExceptionAsync(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            error = "InternalError",
            message,
            timestamp = DateTime.UtcNow
        };

        return context.Response.WriteAsJsonAsync(response);
    }

    private static bool IsDatabaseConnectivityError(Exception ex)
    {
        return ex.Message.Contains("network-related") ||
               ex.InnerException is SocketException ||
               ex is TaskCanceledException;
    }
}