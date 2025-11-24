using IdentityServer.Models;
using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<OidcProvider> OidcProviders => Set<OidcProvider>();
    public DbSet<SamlProvider> SamlProviders => Set<SamlProvider>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure OIDC Provider
        builder.Entity<OidcProvider>(entity =>
        {
            entity.ToTable("DynamicOidcProviders");
            entity.HasIndex(e => e.Scheme).IsUnique();
            entity.HasIndex(e => e.Enabled); // For filtering enabled providers
        });

        // Configure SAML Provider
        builder.Entity<SamlProvider>(entity =>
        {
            entity.ToTable("DynamicSamlProviders");
            entity.HasIndex(e => e.Scheme).IsUnique();
            entity.HasIndex(e => e.Enabled); // For filtering enabled providers
        });

        // Configure Custom User Properties
        builder.Entity<ApplicationUser>(entity =>
        {
            // Add indexes for commonly queried fields
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Company);
            entity.HasIndex(e => e.LastLoginAt);
            
            // Configure string length constraints
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PreferredLanguage).HasMaxLength(10);
            entity.Property(e => e.TimeZone).HasMaxLength(100);
            entity.Property(e => e.Company).HasMaxLength(200);
            entity.Property(e => e.JobTitle).HasMaxLength(200);
            entity.Property(e => e.Department).HasMaxLength(200);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
        });
    }
}