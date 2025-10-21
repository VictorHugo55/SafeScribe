using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SafeScribe.Application.Interfaces;
using SafeScribe.Infrastructure.Context;
using SafeScribe.Infrastructure.Middleware;
using SafeScribe.Infrastructure.Security;
using SafeScribe.Infrastructure.Services;

namespace SafeScribe;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("SafeScribeDB"));

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SafeScribe API",
                Version = "v1",
                Description = "API para autenticação e gestão segura de notas com JWT."
            });

            // 🔐 Configuração do botão "Authorize"
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT assim: **Bearer {seu_token_aqui}**"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        
        
        // Configuração do JWT
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Parâmetros de validação do Token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ✅ Garante que a assinatura do token é válida
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // ✅ Garante que o token vem de um emissor confiável
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    // ✅ Garante que o token é destinado ao público correto
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    // ✅ Garante que o token ainda não expirou
                    ValidateLifetime = true,

                    // ✅ Define que o tempo do sistema é usado para validar expiração
                    ClockSkew = TimeSpan.Zero 
                };
            });

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();
        builder.Services.AddAuthorization();
        

        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        // ✅ Middleware de Blacklist precisa vir antes do UseAuthentication
        app.UseMiddleware<JwtBlacklistMiddleware>();
        
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}