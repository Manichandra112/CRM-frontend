using CRM_Backend.DTOs.Auth;
using CRM_Backend.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace CRM_Backend.Services.Implementations;

public class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;

    public GeoLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient
            ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string?> ResolveAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) ||
            ipAddress == "::1" ||
            ipAddress.StartsWith("127."))
        {
            return "Localhost";
        }

        if (!IPAddress.TryParse(ipAddress, out _))
            return null;

        try
        {
            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>(
                $"https://ip-api.com/json/{ipAddress}"
            );

            if (response == null || response.Status != "success")
                return null;

            return string.Join(", ",
                new[] { response.City, response.RegionName, response.Country }
                .Where(x => !string.IsNullOrWhiteSpace(x))
            );
        }
        catch
        {
            return null;
        }
    }
}
