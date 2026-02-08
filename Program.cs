using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

const string BaseUrl = "https://mco.nalog.ru/api/v1";
const int PageSize = 10;
const string OutputDir = "receipts";

if (args.Length > 0 && args[0] == "build")
    return BuildData();

return await Download(args);

int BuildData()
{
    var mappingPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "store_mapping.json");
    if (!File.Exists(mappingPath))
        mappingPath = "store_mapping.json";

    var storeMapping = JsonSerializer.Deserialize<Dictionary<string, string>>(
        File.ReadAllText(mappingPath)) ?? [];

    var files = Directory.GetFiles(OutputDir, "*.json");
    var records = new List<ReceiptRecord>();

    foreach (var file in files)
    {
        var raw = File.ReadAllText(file, Encoding.UTF8).TrimStart('\uFEFF');
        var receipt = JsonSerializer.Deserialize<Receipt>(raw);
        if (receipt == null)
            continue;

        if (receipt.PrepaidSum != null && receipt.TotalSum != null && receipt.PrepaidSum >= receipt.TotalSum)
            continue;

        if (receipt.Items == null || !receipt.Items.Any(i => !string.IsNullOrEmpty(i.Name)))
            continue;

        var store = (receipt.User != null && storeMapping.TryGetValue(receipt.User, out var s1) ? s1 : null)
            ?? (receipt.RetailPlace != null && storeMapping.TryGetValue(receipt.RetailPlace, out var s2) ? s2 : null)
            ?? receipt.User ?? receipt.RetailPlace ?? "Unknown";

        foreach (var item in receipt.Items)
        {
            if (string.IsNullOrEmpty(item.Name))
                continue;

            records.Add(new ReceiptRecord(item.Name, item.Sum, item.Quantity, item.Price, receipt.DateTime, store));
        }
    }

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    var json = JsonSerializer.Serialize(records, jsonOptions);
    var output = $"const RECEIPT_DATA = {json};\n";
    File.WriteAllText("data.js", output, Encoding.UTF8);

    Console.WriteLine($"Processed {files.Length} files, extracted {records.Count} item records.");
    return 0;
}

async Task<int> Download(string[] args)
{
    var token = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("NALOG_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        Console.Error.WriteLine("Usage: ReceiptDownloader <bearer_token>  — download receipts");
        Console.Error.WriteLine("       ReceiptDownloader build           — build data.js from receipts");
        Console.Error.WriteLine("  or set NALOG_TOKEN environment variable");
        return 1;
    }

    Directory.CreateDirectory(OutputDir);

    const int maxParallelism = 3;
    const int maxRetries = 3;

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    using var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    int offset = 0;
    int downloaded = 0;
    int skipped = 0;

    Console.WriteLine("Fetching receipts...");

    while (true)
    {
        var listBody = new { limit = PageSize, offset, dateFrom = (string?)null, dateTo = (string?)null, orderBy = "CREATED_DATE:DESC", inn = (string?)null, kktOwner = "" };
        var listResponse = await http.PostAsJsonAsync($"{BaseUrl}/receipt", listBody);
        listResponse.EnsureSuccessStatusCode();

        var listResult = await listResponse.Content.ReadFromJsonAsync<ReceiptListResponse>(jsonOptions);
        if (listResult?.Receipts == null || listResult.Receipts.Count == 0)
            break;

        await Parallel.ForEachAsync(listResult.Receipts,
            new ParallelOptions { MaxDegreeOfParallelism = maxParallelism },
            async (receipt, ct) =>
        {
            var fileId = $"{receipt.FiscalDriveNumber}_{receipt.FiscalDocumentNumber}";
            var filePath = Path.Combine(OutputDir, $"{fileId}.json");

            if (File.Exists(filePath))
            {
                Interlocked.Increment(ref skipped);
                Console.WriteLine($"  SKIP {fileId} (already exists)");
                return;
            }

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var detailBody = new { key = receipt.Key };
                    var detailResponse = await http.PostAsJsonAsync($"{BaseUrl}/receipt/fiscal_data", detailBody, ct);

                    if (detailResponse.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                    {
                        Interlocked.Increment(ref skipped);
                        Console.WriteLine($"  SKIP {fileId} (422 Unprocessable Entity)");
                        return;
                    }

                    detailResponse.EnsureSuccessStatusCode();

                    var detailJson = await detailResponse.Content.ReadAsStringAsync(ct);
                    var detailDoc = JsonSerializer.Deserialize<JsonElement>(detailJson);
                    var formatted = JsonSerializer.Serialize(detailDoc, jsonOptions);

                    await File.WriteAllTextAsync(filePath, formatted, Encoding.UTF8, ct);
                    Interlocked.Increment(ref downloaded);
                    Console.WriteLine($"  OK   {fileId} — {receipt.TotalSum} — {receipt.KktOwner}");
                    return;
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                    Console.WriteLine($"  RETRY {fileId} (attempt {attempt}/{maxRetries}: {ex.Message}), waiting {delay.TotalSeconds}s...");
                    await Task.Delay(delay, ct);
                }
            }
        });

        if (listResult.HasMore != true)
            break;

        offset += PageSize;
    }

    Console.WriteLine($"\nDone. Downloaded: {downloaded}, Skipped: {skipped}");
    return 0;
}

record ReceiptListResponse(
    [property: JsonPropertyName("receipts")] List<ReceiptListItem> Receipts,
    [property: JsonPropertyName("hasMore")] bool? HasMore
);

record ReceiptListItem(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("fiscalDocumentNumber")] string FiscalDocumentNumber,
    [property: JsonPropertyName("fiscalDriveNumber")] string FiscalDriveNumber,
    [property: JsonPropertyName("totalSum")] string TotalSum,
    [property: JsonPropertyName("kktOwner")] string KktOwner,
    [property: JsonPropertyName("createdDate")] string CreatedDate
);

record Receipt
{
    [JsonPropertyName("user")] public string? User { get; init; }
    [JsonPropertyName("retailPlace")] public string? RetailPlace { get; init; }
    [JsonPropertyName("dateTime")] public string? DateTime { get; init; }
    [JsonPropertyName("totalSum")] public decimal? TotalSum { get; init; }
    [JsonPropertyName("prepaidSum")] public decimal? PrepaidSum { get; init; }
    [JsonPropertyName("items")] public List<ReceiptItem>? Items { get; init; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
}

record ReceiptItem
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("sum")] public decimal Sum { get; init; }
    [JsonPropertyName("quantity")] public double Quantity { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
}

record ReceiptRecord(string Name, decimal Sum, double Quantity, decimal Price, string? Date, string Store);
