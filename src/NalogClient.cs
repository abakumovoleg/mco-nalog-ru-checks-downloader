using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReceiptDownloader;

class NalogClient(string token) : IDisposable
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
     };
     
    const string BaseUrl = "https://mco.nalog.ru/api/v1";

    readonly HttpClient _http = CreateHttpClient(token);

    public async Task<ReceiptListResponse?> ListReceipts(int limit, int offset, CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync($"{BaseUrl}/receipt", new ReceiptListRequest(limit, offset), ct);
        
        Console.WriteLine($"  Listed receipts with offset {offset}, limit {limit} — Status: {response.StatusCode}");
        
        if(!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Failed to list receipts: {response.StatusCode} — {error}");
        }

        return await response.Content.ReadFromJsonAsync<ReceiptListResponse>(jsonOptions, ct);
    }

    /// <summary>Returns null when the server responds with 422 (receipt unavailable).</summary>
    public async Task<Receipt?> GetReceiptDetail(string key, CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync($"{BaseUrl}/receipt/fiscal_data", new { key }, ct);

        if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            return null;

        if(!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Failed to get receipt detail: {response.StatusCode} — {error}");
        }

        return await response.Content.ReadFromJsonAsync<Receipt>(jsonOptions, ct);
    }

    static HttpClient CreateHttpClient(string token)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return http;
    }

    public void Dispose() => _http.Dispose();
}
