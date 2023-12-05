using Microsoft.AspNetCore.Diagnostics;
using SurveyAPI.Exceptions;

namespace Survey.API.Exceptions
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string errorMessage = "Something Went Wrong";
            int statusCode;

            switch (exception)
            {
                case ForbidException:
                    statusCode = 403;
                    break;
                case BadRequestException badHttpRequest:
                    statusCode = 400;
                    errorMessage = badHttpRequest.Message;
                    break;
                case NotFoundException notFoundException:
                    statusCode = 404;
                    errorMessage = notFoundException.Message;
                    break;
                default:
                    statusCode = 500;
                    break;
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(errorMessage);
            return true;
        }
    }
}
