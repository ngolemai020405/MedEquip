using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
namespace WebClient.Services;
public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _accessor;
    public ApiClient(HttpClient http, IHttpContextAccessor accessor)
    {
        _http = http;
        _accessor = accessor;
    }
    private void GanToken()
    {
        var token = _accessor.HttpContext?.Session.GetString("Token");
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
    public async Task<T?> GetAsync<T>(string url)
    {
        GanToken();
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return default;
        var json = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }
    public async Task<(bool ThanhCong, string? Loi, T? DuLieu)> PostAsync<T>(string url, object body)
    {
        GanToken();
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var res = await _http.PostAsync(url, content);
        var json = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode) return (false, json, default);
        return (true, null, JsonConvert.DeserializeObject<T>(json));
    }
    public async Task<(bool ThanhCong, string? Loi)> PutAsync(string url, object body)
    {
        GanToken();
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var res = await _http.PutAsync(url, content);
        var json = await res.Content.ReadAsStringAsync();
        return res.IsSuccessStatusCode ? (true, null) : (false, json);
    }
}