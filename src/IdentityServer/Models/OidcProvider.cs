using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

/// <summary>
/// Configuration for an OIDC (OpenID Connect) dynamic authentication provider
/// </summary>
public class OidcProvider : DynamicProvider
{
    public OidcProvider()
    {
        ProviderType = "OIDC";
    }
    
    /// <summary>
    /// Authority URL of the OIDC provider
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Authority { get; set; } = string.Empty;
    
    /// <summary>
    /// Client ID for the application
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Client Secret for the application
    /// </summary>
    [StringLength(500)]
    public string? ClientSecret { get; set; }
    
    /// <summary>
    /// Response type (code, token, id_token, etc.)
    /// </summary>
    [StringLength(100)]
    public string ResponseType { get; set; } = "code";
    
    /// <summary>
    /// Scopes to request (space or comma separated)
    /// </summary>
    [StringLength(500)]
    public string Scopes { get; set; } = "openid profile";
    
    /// <summary>
    /// Callback path for authentication response
    /// </summary>
    [StringLength(200)]
    public string CallbackPath { get; set; } = "/signin-oidc";
    
    /// <summary>
    /// Whether to get claims from the user info endpoint
    /// </summary>
    public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
    
    /// <summary>
    /// Whether to save tokens
    /// </summary>
    public bool SaveTokens { get; set; } = true;
    
    /// <summary>
    /// Metadata address (if different from authority)
    /// </summary>
    [StringLength(500)]
    public string? MetadataAddress { get; set; }
    
    /// <summary>
    /// Whether to require HTTPS for metadata
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
