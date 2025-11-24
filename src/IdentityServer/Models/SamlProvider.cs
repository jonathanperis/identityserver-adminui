using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

/// <summary>
/// Configuration for a SAML2P dynamic authentication provider
/// </summary>
public class SamlProvider : DynamicProvider
{
    public SamlProvider()
    {
        ProviderType = "SAML";
    }
    
    /// <summary>
    /// Service Provider Entity ID
    /// </summary>
    [Required]
    [StringLength(500)]
    public string SpEntityId { get; set; } = string.Empty;
    
    /// <summary>
    /// Identity Provider Entity ID
    /// </summary>
    [Required]
    [StringLength(500)]
    public string IdpEntityId { get; set; } = string.Empty;
    
    /// <summary>
    /// Identity Provider Single Sign-On URL
    /// </summary>
    [Required]
    [StringLength(500)]
    public string IdpSingleSignOnUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Identity Provider metadata URL (alternative to manual configuration)
    /// </summary>
    [StringLength(500)]
    public string? IdpMetadataUrl { get; set; }
    
    /// <summary>
    /// Assertion Consumer Service URL/Path
    /// </summary>
    [StringLength(200)]
    public string AcsPath { get; set; } = "/saml/acs";
    
    /// <summary>
    /// Identity Provider certificate (Base64 encoded)
    /// </summary>
    public string? IdpCertificate { get; set; }
    
    /// <summary>
    /// Service Provider certificate (Base64 encoded PFX)
    /// WARNING: In production, encrypt this field or use Azure Key Vault
    /// </summary>
    public string? SpCertificate { get; set; }
    
    /// <summary>
    /// Service Provider certificate password
    /// WARNING: In production, encrypt this field or use Azure Key Vault
    /// </summary>
    [StringLength(200)]
    public string? SpCertificatePassword { get; set; }
    
    /// <summary>
    /// Whether to sign authentication requests
    /// </summary>
    public bool SignAuthenticationRequests { get; set; } = false;
    
    /// <summary>
    /// Whether assertions must be signed
    /// </summary>
    public bool WantAssertionsSigned { get; set; } = true;
    
    /// <summary>
    /// Name ID format
    /// </summary>
    [StringLength(200)]
    public string NameIdFormat { get; set; } = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
    
    /// <summary>
    /// Binding type (POST, Redirect, etc.)
    /// </summary>
    [StringLength(50)]
    public string BindingType { get; set; } = "POST";
}
