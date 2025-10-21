namespace SafeScribe.Domain.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Chave estrangeira
    public Guid UserId { get; set; }

    // Navegação reversa
    public User? User { get; set; }
}