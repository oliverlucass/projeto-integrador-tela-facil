using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

public class SupabaseAuth
{
    private readonly HttpClient _httpClient;
    private readonly string _authUrl;
    private readonly string _apiKey;

    public SupabaseAuth(string projectUrl, string apiKey)
    {
        _httpClient = new HttpClient();
        _authUrl = $"{projectUrl}/auth/v1/token?grant_type=password";
        _apiKey = apiKey;
    }

    public async Task<string> LoginAndGetAccessTokenAsync(string email, string password)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var loginData = new
        {
            email = email,
            password = password
        };

        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_authUrl, content);

        var responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var json = JsonDocument.Parse(responseData);
            var token = json.RootElement.GetProperty("access_token").GetString();
            return token;
        }
        else
        {
            throw new Exception("Erro ao autenticar: " + responseData);
        }
    }
}