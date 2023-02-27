using NLog;
using Ozon.Route256.Five.OrderService.DTO;
using System.Net;

namespace Ozon.Route256.Five.OrderService.Midlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsJsonAsync(new ErrorDetails(context.Response.StatusCode, "Internal Server Error"));
    }
}
