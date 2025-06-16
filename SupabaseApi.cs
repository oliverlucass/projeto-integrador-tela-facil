using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SupabaseApi
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public SupabaseApi(string baseUrl, string jwtToken, string apiKey)
    {
        _baseUrl = baseUrl;  
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<TelaFacilLauncher.Shortcut>> GetAtalhosAsync(string nome_atalho = null)
    {
        string url = _baseUrl + "/rest/v1/atalhos";

        if (!string.IsNullOrWhiteSpace(nome_atalho))
        {
            url += "?nome_atalho=eq." + Uri.EscapeDataString(nome_atalho) + "&select=*";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();

        // Converte o JSON para uma lista de objetos Atalho
        var atalhos = JsonConvert.DeserializeObject<List<TelaFacilLauncher.Shortcut>>(responseData);

        return atalhos;
    }

    public async Task<string> InserirAtalhoAsync(string nome, string caminho)
    {
        var novo_atalho = new 
        {
            nome_atalho = nome,
            caminho_atalho = caminho
        };
        
        string url = _baseUrl + "/rest/v1/atalhos";

        string json = System.Text.Json.JsonSerializer.Serialize(novo_atalho);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return responseData;
    }

    public async Task<string> DeletarAtalhoAsync(string nome_atalho)
    {
        string url = _baseUrl + "/rest/v1/atalhos?nome_atalho=eq." + nome_atalho;

        var request = new HttpRequestMessage(HttpMethod.Delete, url);

        request.Headers.Add("Prefer", "return=representation");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return responseData; 
    }

    public async Task<string> AtualizarAtalhoAsync(string nomeOriginal, string novoNome, string novoCaminho)
    {
        string url = _baseUrl + "/rest/v1/atalhos?nome_atalho=eq." + Uri.EscapeDataString(nomeOriginal);

        var dadosAtualizados = new
        {
            nome_atalho = novoNome,
            caminho_atalho = novoCaminho
        };

        string json = System.Text.Json.JsonSerializer.Serialize(dadosAtualizados);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, url);
        request.Content = content;

        request.Headers.Add("Prefer", "return=representation");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return responseData;
    }
}