using System.IdentityModel.Tokens.Jwt;
using SafeScribe.Application.Interfaces;

namespace SafeScribe.Infrastructure.Middleware;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService tokenBlacklistService)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();

            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var jti = jwtToken.Id;

                if (tokenBlacklistService.IsTokenBlacklisted(jti))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido ou expirado (blacklist).");
                    return; // interrompe a pipeline
                }
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token inválido.");
                return;
            }
        }

        // Continua para os próximos middlewares
        await _next(context);
    }
}
