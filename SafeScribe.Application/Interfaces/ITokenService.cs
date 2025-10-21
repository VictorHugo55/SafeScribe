using SafeScribe.Application.DTOs;
using SafeScribe.Domain.Entities;

namespace SafeScribe.Application.Interfaces;

public interface ITokenService
{
    Task<User?> RegisterAsync(UserRegisterDto dto);
    Task<string?> LoginAsync(LoginRequestDto dto);
}