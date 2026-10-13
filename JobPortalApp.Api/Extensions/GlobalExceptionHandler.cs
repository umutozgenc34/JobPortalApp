using JobPortalApp.Shared.Responses;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace JobPortalApp.Api.Extensions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is UnauthorizedAccessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(ServiceResult.Fail("Unauthorized.", HttpStatusCode.Unauthorized), cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        var message = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(ServiceResult.Fail(message, HttpStatusCode.InternalServerError), cancellationToken);
        return true;
    }
}
