using System.Net;
using System.Text.Json;
using Dsw2026Ej15.Domain.Exceptions;

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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = "Ocurrio un error inesperado al ejecutar la solicitud.";
            context.Response.ContentType = "application/json";

            if (exception is ValidationException ve)
            {
                status = HttpStatusCode.BadRequest;
                message = ve.Message;
            }
            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(result);
        }
    }
}
