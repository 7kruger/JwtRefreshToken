using Microsoft.EntityFrameworkCore;
using JwtRefreshToken.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace JwtRefreshToken.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public virtual DbSet<UserRefreshToken> UserRefreshToken { get; set; }
}
