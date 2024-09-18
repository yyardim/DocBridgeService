using DocBridgeService.Models;
using Newtonsoft.Json;
using System.Text;

namespace DocBridgeService.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<RepoCentralResponse> SendToRepoCentralAsync(FileData fileData)
    {
        var jsonContent = JsonConvert.SerializeObject(fileData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://repocentral/api/files", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<RepoCentralResponse>(responseContent);
    }
}
