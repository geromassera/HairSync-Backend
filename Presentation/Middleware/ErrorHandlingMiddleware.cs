using Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Presentation.Middleware
{ 
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await HandleExceptionsAsync(context, ex);
            }
        }

        private static Task HandleExceptionsAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;

            switch(exception)
            {
                case ValidException validException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = validException.Message;
                    break;

                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = badRequestException.Message;
                    break;

                case ForbiddenAccessException forbiddenAccessException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = forbiddenAccessException.Message;
                    break;

                case UnauthorizedException unauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedException.Message;
                    break;

                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                    break;

                case ConflictException conflictException:
                    statusCode = HttpStatusCode.Conflict;
                    message = conflictException.Message;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Ocurrio un error inesperado.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = new
            {
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
