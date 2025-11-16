using System.Net.Http.Json;
using Microsoft.Extensions.Options;

public class TurnstileValidator
{
    private readonly HttpClient _httpClient;
    private readonly TurnstileOptions _options;

    public TurnstileValidator(
        IHttpClientFactory httpClientFactory,
        IOptions<TurnstileOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("Turnstile");
        _options = options.Value;
    }

    public async Task<bool> IsValidAsync(string? token, string? remoteIp)
    {
        if (!_options.Enabled)
            return true;

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("secret", _options.SecretKey),
            new KeyValuePair<string, string>("response", token),
            new KeyValuePair<string, string>("remoteip", remoteIp ?? string.Empty),
        });

        var response = await _httpClient.PostAsync("/turnstile/v0/siteverify", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>();
        return result?.Success == true;
    }
}

public class TurnstileVerifyResponse
{
    public bool Success { get; set; }
    public string[] Error_Codes { get; set; } = Array.Empty<string>();
}