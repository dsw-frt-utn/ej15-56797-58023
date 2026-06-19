using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Dsw2026Ej15.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pasa el control al siguiente componente en el pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción generada en controladores o capas inferiores
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new
            {
                Message = exception is ValidationException
                    ? exception.Message
                    : "Ocurrió un problema interno en el servidor."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
