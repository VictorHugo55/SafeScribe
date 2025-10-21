using Microsoft.EntityFrameworkCore;
using SafeScribe.Domain.Entities;

namespace SafeScribe.Infrastructure.Context;

public class AppDbContext :  DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Note> Notes => Set<Note>();

}