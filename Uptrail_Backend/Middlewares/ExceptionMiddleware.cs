using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Uptrail_Backend.Common.Responses;
using Uptrail_Backend.Exceptions;

namespace Uptrail_Backend.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, exception middleware cannot write a response.");
                    throw;
                }

                context.Response.ContentType = "application/json";

                switch (ex)
                {
                    case NotFoundException notFoundException:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(notFoundException.Message));
                        break;

                    case KeyNotFoundException keyNotFoundException:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(keyNotFoundException.Message));
                        break;

                    case BadRequestException badRequestException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(badRequestException.Message));
                        break;

                    case ArgumentException argumentException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(argumentException.Message));
                        break;

                    case UnauthorizedAccessException unauthorizedException:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(unauthorizedException.Message));
                        break;

                    case SecurityTokenException securityTokenException:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>(securityTokenException.Message));
                        break;

                    case DbUpdateConcurrencyException:
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>("The record was modified by another operation. Please refresh and try again."));
                        break;

                    case DbUpdateException dbUpdateException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        var errorDetail = dbUpdateException.InnerException?.Message ?? dbUpdateException.Message;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>($"Database operation failed: {errorDetail}"));
                        break;

                    default:
                        _logger.LogError(ex, "An unhandled exception occurred.");
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(ApiResponseFactory.ErrorResponse<object?>("An unexpected internal server error occurred."));
                        break;
                }
            }
        }
    }
}
