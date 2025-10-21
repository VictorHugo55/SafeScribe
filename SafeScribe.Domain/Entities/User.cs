namespace SafeScribe.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Senha protegida com hash (ex: BCrypt)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Função/perfil do usuário (Leitor, Editor, Admin)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    // Navegação — Um usuário pode ter várias notas
    public ICollection<Note> Notes { get; set; } = new List<Note>();    
}