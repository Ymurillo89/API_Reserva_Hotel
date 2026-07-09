using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TuProyecto.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
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

        // Mapeamos el tipo de excepción a un código HTTP específico
        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,         // 400
            InvalidOperationException => (int)HttpStatusCode.Conflict,   // 409 
            KeyNotFoundException => (int)HttpStatusCode.NotFound,        // 404
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,// 403
            _ => (int)HttpStatusCode.InternalServerError                 // 500
        };

        // Creamos una respuesta anónima limpia y bonita
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Mensaje = exception.Message,
            // Solo mostramos detalles técnicos si estamos en desarrollo o si es un error 500 no controlado
            Detalle = context.Response.StatusCode == 500 ? "Ocurrió un error interno en el servidor." : null
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        
        return context.Response.WriteAsync(jsonResponse);
    }
}
