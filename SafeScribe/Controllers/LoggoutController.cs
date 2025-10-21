using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeScribe.Application.Interfaces;
using SafeScribe.Domain.Entities;
using SafeScribe.Infrastructure.Context;

namespace SafeScribe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggoutController : ControllerBase
    {   
        
            private readonly ITokenBlacklistService _tokenBlacklistService;

            public LoggoutController(ITokenBlacklistService tokenBlacklistService)
            {
                _tokenBlacklistService = tokenBlacklistService;
            }

            /// <summary>
            /// Realiza o logout do usuário invalidando o token atual.
            /// </summary>
            [HttpPost("Logout")]
            [Authorize]
            public IActionResult Logout()
            {
                // Obtém o token do cabeçalho de autorização
                var authHeader = Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return BadRequest(new { message = "Token inválido ou ausente." });

                var token = authHeader["Bearer ".Length..].Trim();

                try
                {
                    // Decodifica o token para obter o ID único (jti)
                    var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                    var jti = jwtToken.Id;

                    // Adiciona à blacklist
                    _tokenBlacklistService.AddTokenToBlacklist(jti);

                    return Ok(new { message = "Logout realizado com sucesso. Token invalidado." });
                }
                catch
                {
                    return BadRequest(new { message = "Falha ao processar o token." });
                }
            }
        
        
    }
    
}
