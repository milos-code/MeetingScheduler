using MeetingScheduler.Api.LogingMiddleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.ExceptionMiddleware
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        //private readonly ILogger<GlobalExceptionHandler> _logger;

        //public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        //{
        //    _logger = logger;
        //}

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            LogHandlingMiddleware.LogError(
                exception,
                "Exception ocurred: ");

            var problemDetials = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error"
            };

            httpContext.Response.StatusCode = problemDetials.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetials, cancellationToken);

            return true;
        }
    }
}
