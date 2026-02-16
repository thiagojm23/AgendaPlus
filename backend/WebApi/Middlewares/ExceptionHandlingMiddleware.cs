using System.Net;
using System.Text.Json;
using FluentValidation;

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
            title = "Um ou mais erros de validação ocorreram.",
            status = 400,
            errors
        };

        var json = JsonSerializer.Serialize(response, _jsonSerializerOptions);

        await context.Response.WriteAsync(json);
    }
}