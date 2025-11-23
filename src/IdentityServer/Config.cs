using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Duende.IdentityServer;

namespace IdentityServer;

public static class Config
{
    public static List<TestUser> Users
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return [
              new() {
          SubjectId = "818727",
            Username = "alice",
            Password = "alice",
            Claims = {
              new Claim(JwtClaimTypes.Name, "Alice Smith"),
              new Claim(JwtClaimTypes.GivenName, "Alice"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
        },
        new() {
          SubjectId = "88421113",
            Username = "bob",
            Password = "bob",
            Claims = {
              new Claim(JwtClaimTypes.Name, "Bob Smith"),
              new Claim(JwtClaimTypes.GivenName, "Bob"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "user"),
              new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
        }
            ];
        }
    }

    public static IEnumerable<IdentityResource> IdentityResources => [
      new IdentityResources.OpenId(),
    new IdentityResources.Profile(),
    new IdentityResource {
      Name = "role",
        UserClaims = ["role"]
    }
    ];

    public static IEnumerable<ApiScope> ApiScopes => [
      new ApiScope("weatherapi.read"),
    new ApiScope("weatherapi.write"),
  ];

    public static IEnumerable<ApiResource> ApiResources => [
      new ApiResource("weatherapi") {
      Scopes = ["weatherapi.read", "weatherapi.write"],
        ApiSecrets = [new("ScopeSecret".Sha256())],
        UserClaims = ["role"]
    }
    ];

    public static IEnumerable<Client> Clients => [
      // m2m client credentials flow client
      new Client {
      ClientId = "m2m.client",
        ClientName = "Client Credentials Client",

        AllowedGrantTypes = GrantTypes.ClientCredentials,
        ClientSecrets = {
          new Secret("92411407-C4AC-4C3E-87E6-30DA6C3BCCA2".Sha256())
        },

        AllowedScopes = {
          "weatherapi.read",
          "weatherapi.write"
        }
    },

    // interactive client using code flow + pkce
    new Client {
      ClientId = "interactive",
        ClientSecrets = {
          new Secret("AD6E3240-EDAE-4C59-B35C-10E2033CFAAB".Sha256())
        },

        AllowedGrantTypes = GrantTypes.Code,

        RedirectUris = {
          "https://localhost:5444/signin-oidc"
        },
        FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
        PostLogoutRedirectUris = {
          "https://localhost:5444/signout-callback-oidc"
        },

        AllowOfflineAccess = true,
        AllowedScopes = {
          "openid",
          "profile",
          "weatherapi.read"
        },
        RequirePkce = true,
        //RequireConsent = true,
        AllowPlainTextPkce = false
    },
  ];
}

//curl -X POST -H "content-type: application/x-www-form-urlencoded" -H "Cache-Control: no-cache" -d 'client_id=m2m.client&scope=weatherapi.read&client_secret=AD6E3240-EDAE-4C59-B35C-10E2033CFAAB&grant_type=client_credentials' "https://localhost:5443/connect/token"