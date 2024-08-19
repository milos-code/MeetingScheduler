using MeetingScheduler.Api.LogingMiddleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentRecords.Bussines.Exceptions;

namespace MeetingScheduler.Api.ExceptionMiddleware
{
    internal sealed class BadRequestExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ApiException badHttpRequestException)
            {
                return false;
            }

            LogHandlingMiddleware.LogError(
                badHttpRequestException,
                "Exception ocurred: ");

            var problemDetials = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = badHttpRequestException.Message
            };

            httpContext.Response.StatusCode = problemDetials.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetials, cancellationToken);

            return true;
        }
    }
}
