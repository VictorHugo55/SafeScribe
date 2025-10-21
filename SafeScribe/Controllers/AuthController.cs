using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeScribe.Application.DTOs;
using SafeScribe.Application.Interfaces;
using SafeScribe.Domain.Entities;
using SafeScribe.Infrastructure.Context;

namespace SafeScribe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public AuthController(ITokenService tokenService, ITokenBlacklistService tokenBlacklistService)
        {
            _tokenService = tokenService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        /// <summary>
        /// Registra um novo usuário.
        /// </summary>
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] UserRegisterDto dto)
        {
            var user = await _tokenService.RegisterAsync(dto);

            if (user == null)
                return BadRequest(new { message = "Usuário já existe." });

            return CreatedAtAction(nameof(Registrar), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Role
            });
        }

        /// <summary>
        /// Realiza login e retorna um token JWT válido.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var token = await _tokenService.LoginAsync(dto);

            if (token == null)
                return Unauthorized(new { message = "Credenciais inválidas." });

            return Ok(new { token });
        }
        /*
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token não encontrado." });

            _tokenBlacklistService.AddToken(token);

            return Ok(new { message = "Logout realizado com sucesso." });
        }
        */
    }
}
