using IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
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
        });

        // Configure SAML Provider
        builder.Entity<SamlProvider>(entity =>
        {
            entity.ToTable("DynamicSamlProviders");
            entity.HasIndex(e => e.Scheme).IsUnique();
        });
    }
}