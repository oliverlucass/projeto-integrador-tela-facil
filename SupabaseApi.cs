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
        _baseUrl = baseUrl;  // ex: https://<seu-projeto>.supabase.co/rest/v1/
        _httpClient = new HttpClient();

        // Header obrigatório para autenticação da API
        _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

        // Header Authorization com o JWT obtido no login
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        // Aceitar JSON
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<TelaFacilLauncher.Shortcut>> GetAtalhosAsync()
    {
        string url = _baseUrl + "/rest/v1/atalhos";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Cabeçalhos padrão Supabase
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
        return responseData; // Normalmente retorna o objeto inserido
    }

    public async Task<string> DeletarAtalhoAsync(string nome_atalho)
    {
        //string url = $"{_baseUrl}/rest/v1/atalhos?id=eq.{nome_atalho}";
        string url = _baseUrl + "/rest/v1/atalhos?nome_atalho=eq." + nome_atalho;

        var request = new HttpRequestMessage(HttpMethod.Delete, url);

        // Importante: para que o Supabase retorne dados após o DELETE (opcional)
        request.Headers.Add("Prefer", "return=representation");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return responseData; // Pode conter os dados deletados, dependendo da configuração
    }
}