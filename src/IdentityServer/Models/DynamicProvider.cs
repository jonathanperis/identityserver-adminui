using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

/// <summary>
/// Base class for dynamic authentication providers
/// </summary>
public abstract class DynamicProvider
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Unique scheme name for the authentication provider
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Scheme { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name shown to users
    /// </summary>
    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the provider is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Type of the provider (OIDC, SAML, etc.)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ProviderType { get; set; } = string.Empty;
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? Updated { get; set; }
}
