using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Exceptions;

namespace Uptrail_Backend.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            Console.WriteLine("no errors yet");
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("ExceptionMiddleware invoked");
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case NotFoundException notFoundException:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(new { message = notFoundException.Message });
                        break;

                    case BadRequestException badRequestException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new { message = badRequestException.Message });
                        break;

                    case UnauthorizedAccessException unauthorizedException:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { message = unauthorizedException.Message });
                        break;

                    case DbUpdateConcurrencyException:
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsJsonAsync(new { message = "The record you attempted to edit was modified by another user after you got the original value. The edit operation was canceled." });
                        break;

                    case DbUpdateException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                        break;

                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                        break;
                }
            }
        }
    }

}
