using Microsoft.AspNetCore.Http;

namespace FSH.Framework.Infrastructure.Cors;
public class DevelopmentCorsMiddleware
{
    private readonly RequestDelegate _next;

    public DevelopmentCorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();
        if (!string.IsNullOrEmpty(origin) && origin.StartsWith("https://localhost"))
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        }

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 204;
            await context.Response.CompleteAsync();
            return;
        }

        await _next(context);
    }
}
