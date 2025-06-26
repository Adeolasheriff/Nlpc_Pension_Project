

using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using OpenQA.Selenium;

namespace Nlpc_Pension_Project.Common;

// ExceptionMiddleware.cs
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
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
        var code = HttpStatusCode.InternalServerError;
        string message;

        switch (exception)
        {
            case ValidationException _:
                code = HttpStatusCode.BadRequest;
                message = "Validation failed.";
                break;
            case NotFoundException _:
                code = HttpStatusCode.NotFound;
                message = "Resource not found.";
                break;
            default:
                message = "An unexpected error occurred.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var result = JsonConvert.SerializeObject(new { error = message });
        return context.Response.WriteAsync(result);
    }
}
