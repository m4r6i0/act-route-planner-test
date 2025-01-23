using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace RoutePlanner.API.Middleware
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
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Mapear os tipos de exceções para os códigos de status HTTP apropriados
            context.Response.StatusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest, // Erro do cliente
                KeyNotFoundException => StatusCodes.Status404NotFound, // Recurso não encontrado
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // Acesso não autorizado
                NotImplementedException => StatusCodes.Status501NotImplemented, // Funcionalidade não implementada
                _ => StatusCodes.Status500InternalServerError // Erro genérico do servidor
            };

            // Preparar uma resposta padronizada para produção
            var response = new
            {
                Message = exception.Message,
                // Expor detalhes de exceção apenas em ambientes de desenvolvimento
                Details = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? exception.StackTrace
                    : null
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
