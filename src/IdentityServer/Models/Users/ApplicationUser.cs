using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models.Users;

/// <summary>
/// Custom application user with extended properties beyond standard Identity schema
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// User's full name (computed property)
    /// </summary>
    public string? FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// User's profile picture URL or path
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// User's address line 1
    /// </summary>
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// User's address line 2
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// User's city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// User's state or province
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// User's postal/zip code
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// User's country
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// User's preferred language/locale
    /// </summary>
    public string? PreferredLanguage { get; set; }
    
    /// <summary>
    /// User's timezone
    /// </summary>
    public string? TimeZone { get; set; }
    
    /// <summary>
    /// Company or organization name
    /// </summary>
    public string? Company { get; set; }
    
    /// <summary>
    /// Job title or position
    /// </summary>
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Department
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the user profile was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Date when the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Custom user metadata stored as JSON
    /// </summary>
    public string? CustomMetadata { get; set; }
}
