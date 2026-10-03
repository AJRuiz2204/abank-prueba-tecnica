using Abank.Api.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Abank.Api.Middleware;

/// <summary>
/// Traduce excepciones a respuestas ProblemDetails
/// </summary>
/// <param name="logger"></param>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        (int status, string title) = exception switch
        {
            ValidationException   => (StatusCodes.Status400BadRequest, "Datos inválidos"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            NotFoundException     => (StatusCodes.Status404NotFound, "No encontrado"),
            ConflictException     => (StatusCodes.Status409Conflict, "Conflicto"),
            _                     => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado");

        ProblemDetails problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == 500 ? "Ocurrió un error inesperado." : exception.Message,
            Instance = context.Request.Path
        };

        if (exception is ValidationException ve)
            problem.Extensions["errors"] = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}
