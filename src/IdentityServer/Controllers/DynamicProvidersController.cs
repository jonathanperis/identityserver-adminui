using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

/// <summary>
/// API controller for managing dynamic authentication providers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DynamicProvidersController : ControllerBase
{
    private readonly IDynamicProviderService _providerService;
    private readonly DynamicAuthenticationSchemeService _schemeService;
    private readonly ILogger<DynamicProvidersController> _logger;

    public DynamicProvidersController(
        IDynamicProviderService providerService,
        DynamicAuthenticationSchemeService schemeService,
        ILogger<DynamicProvidersController> logger)
    {
        _providerService = providerService;
        _schemeService = schemeService;
        _logger = logger;
    }

    #region OIDC Providers

    [HttpGet("oidc")]
    public async Task<ActionResult<IEnumerable<OidcProvider>>> GetOidcProviders()
    {
        var providers = await _providerService.GetAllOidcProvidersAsync();
        return Ok(providers);
    }

    [HttpGet("oidc/{id}")]
    public async Task<ActionResult<OidcProvider>> GetOidcProvider(int id)
    {
        var provider = await _providerService.GetOidcProviderAsync(id);
        if (provider == null)
        {
            return NotFound();
        }
        return Ok(provider);
    }

    [HttpPost("oidc")]
    public async Task<ActionResult<OidcProvider>> CreateOidcProvider(OidcProvider provider)
    {
        try
        {
            var created = await _providerService.CreateOidcProviderAsync(provider);
            _logger.LogInformation("Created OIDC provider {Scheme}", provider.Scheme);
            return CreatedAtAction(nameof(GetOidcProvider), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating OIDC provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("oidc/{id}")]
    public async Task<ActionResult<OidcProvider>> UpdateOidcProvider(int id, OidcProvider provider)
    {
        if (id != provider.Id)
        {
            return BadRequest();
        }

        try
        {
            var updated = await _providerService.UpdateOidcProviderAsync(provider);
            _schemeService.ClearOptionsCache(provider.Scheme);
            _logger.LogInformation("Updated OIDC provider {Scheme}", provider.Scheme);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating OIDC provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("oidc/{id}")]
    public async Task<IActionResult> DeleteOidcProvider(int id)
    {
        try
        {
            var provider = await _providerService.GetOidcProviderAsync(id);
            if (provider == null)
            {
                return NotFound();
            }

            await _providerService.DeleteOidcProviderAsync(id);
            _schemeService.ClearOptionsCache(provider.Scheme);
            _logger.LogInformation("Deleted OIDC provider {Scheme}", provider.Scheme);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting OIDC provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region SAML Providers

    [HttpGet("saml")]
    public async Task<ActionResult<IEnumerable<SamlProvider>>> GetSamlProviders()
    {
        var providers = await _providerService.GetAllSamlProvidersAsync();
        return Ok(providers);
    }

    [HttpGet("saml/{id}")]
    public async Task<ActionResult<SamlProvider>> GetSamlProvider(int id)
    {
        var provider = await _providerService.GetSamlProviderAsync(id);
        if (provider == null)
        {
            return NotFound();
        }
        return Ok(provider);
    }

    [HttpPost("saml")]
    public async Task<ActionResult<SamlProvider>> CreateSamlProvider(SamlProvider provider)
    {
        try
        {
            var created = await _providerService.CreateSamlProviderAsync(provider);
            _logger.LogInformation("Created SAML provider {Scheme}", provider.Scheme);
            return CreatedAtAction(nameof(GetSamlProvider), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SAML provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("saml/{id}")]
    public async Task<ActionResult<SamlProvider>> UpdateSamlProvider(int id, SamlProvider provider)
    {
        if (id != provider.Id)
        {
            return BadRequest();
        }

        try
        {
            var updated = await _providerService.UpdateSamlProviderAsync(provider);
            _logger.LogInformation("Updated SAML provider {Scheme}", provider.Scheme);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SAML provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("saml/{id}")]
    public async Task<IActionResult> DeleteSamlProvider(int id)
    {
        try
        {
            var provider = await _providerService.GetSamlProviderAsync(id);
            if (provider == null)
            {
                return NotFound();
            }

            await _providerService.DeleteSamlProviderAsync(id);
            _logger.LogInformation("Deleted SAML provider {Scheme}", provider.Scheme);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SAML provider");
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DynamicProvider>>> GetAllProviders()
    {
        var providers = await _providerService.GetAllEnabledProvidersAsync();
        return Ok(providers);
    }
}
