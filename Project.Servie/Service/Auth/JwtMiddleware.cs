using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Session.GetString("JWTToken");

        if (token != null)
        {
            _logger.LogInformation("JWT Token found in session.");
            context.Request.Headers.Add("Authorization", $"Bearer {token}");
        }
        else
        {
            _logger.LogWarning("JWT Token not found in session.");
        }

        await _next(context);
    }
}
