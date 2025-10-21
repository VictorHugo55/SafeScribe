using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeScribe.Application.DTOs;
using SafeScribe.Domain.Entities;
using SafeScribe.Infrastructure.Context;

namespace SafeScribe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotasController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // POST /api/v1/notas - Criar
        // ============================
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Create([FromBody] NoteCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = Guid.Parse(userIdClaim),
                CreatedAt = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }

        // ============================
        // GET /api/v1/notas/{id} - Obter
        // ============================
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var note = _context.Notes.FirstOrDefault(n => n.Id == id);
            if (note == null) return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Leitor e Editor só podem acessar suas próprias notas
            if (userRole is "Leitor" or "Editor" && note.UserId.ToString() != userId)
                return Forbid();

            return Ok(note);
        }

    // ============================
    // PUT /api/v1/notas/{id} - Atualizar
    // ============================
    [HttpPut("{id}")]
    [Authorize(Roles = "Editor,Admin")] // Leitor não pode atualizar
    public IActionResult Update(Guid id, [FromBody] NoteUpdateDto dto)
    {
        var note = _context.Notes.FirstOrDefault(n => n.Id == id);
        if (note == null) return NotFound();

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userRole == "Editor" && note.UserId.ToString() != userId)
            return Forbid();

        note.Title = dto.Title;
        note.Content = dto.Content;

        _context.SaveChanges();
        return Ok(note);
    }

    // ============================
    // DELETE /api/v1/notas/{id} - Apagar
    // ============================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Apenas Admin
    public IActionResult Delete(Guid id)
    {
        var note = _context.Notes.FirstOrDefault(n => n.Id == id);
        if (note == null) return NotFound();

        _context.Notes.Remove(note);
        _context.SaveChanges();

        return NoContent();
    }
    }
}
