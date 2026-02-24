using Application.DTOS;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.Response.HasStarted)
            {
                await HandleUnauthorizedAsync(context);
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden && !context.Response.HasStarted)
            {
                await HandleForbiddenAsync(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleUnauthorizedAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Failure("Authentication failed. Please log in to access this resource.");
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleForbiddenAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Failure("You do not have permission to perform this action.");
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred. Please try again later.";
        List<string>? errors = null;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = validationEx.Message;
                errors = validationEx.Errors != null && validationEx.Errors.Count > 0 
                         ? validationEx.Errors 
                         : null;
                break;
            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Authentication failed. Please check your credentials.";
                break;
            case KeyNotFoundException keyNotFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = keyNotFoundEx.Message;
                break;
            default:
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var response = ApiResponse<object>.Failure(message, errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
