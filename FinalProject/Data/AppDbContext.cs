using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Exhibition> Exhibitions { get; set; }
    public DbSet<Painting> Paintings { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Relationship between ApplicationUser (Artist) and Paintings
        builder.Entity<Painting>()
            .HasOne(p => p.User)  // A painting is owned by one artist
            .WithMany(u => u.Paintings)  // An artist can own many paintings
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // Deleting user will delete their paintings (or you can adjust this)

        // Relationship between ApplicationUser (Artist) and Exhibitions
        builder.Entity<Exhibition>()
            .HasOne(e => e.Artist)  // Exhibition is associated with one artist
            .WithMany(a => a.Exhibitions)  // An artist can have many exhibitions
            .HasForeignKey(e => e.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);  // Deleting artist will delete exhibitions (you can adjust this too)
    }
    
}