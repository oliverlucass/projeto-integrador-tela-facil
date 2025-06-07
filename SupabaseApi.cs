using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<string> GetProdutosAsync()
    {
        // Endpoint da tabela 'produtos'
        string url = _baseUrl + "/rest/v1/atalhos";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return responseData;  // JSON com os dados da tabela
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
}