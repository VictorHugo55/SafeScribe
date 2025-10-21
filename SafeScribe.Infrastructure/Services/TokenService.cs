using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SafeScribe.Application.DTOs;
using SafeScribe.Application.Interfaces;
using SafeScribe.Domain.Entities;
using SafeScribe.Infrastructure.Context;

namespace SafeScribe.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public TokenService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    /// <summary>
    /// Registra um novo usuário com senha criptografada.
    /// </summary>
    public async Task<User?> RegisterAsync(UserRegisterDto dto)
    {
        // Verifica se o usuário já existe
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return null;

        // Gera o hash seguro da senha
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Autentica o usuário e gera o token JWT se as credenciais forem válidas.
    /// </summary>
    public async Task<string?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
            return null;

        // Verifica o hash da senha
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // Gera o token JWT
        return GenerateJwtToken(user);
    }

    /// <summary>
    /// Cria o JWT com claims (UserId, Role e Jti).
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ✅ Adicionado
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}