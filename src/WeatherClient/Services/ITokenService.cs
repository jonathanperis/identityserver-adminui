using IdentityModel.Client;

namespace WeatherClient.Services;

public interface ITokenService
{
    Task<TokenResponse> GetToken(string scope);
}
