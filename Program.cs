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
    var records = new List<object>();

    foreach (var file in files)
    {
        var raw = File.ReadAllText(file, Encoding.UTF8).TrimStart('\uFEFF');
        var receipt = JsonSerializer.Deserialize<JsonElement>(raw);

        var prepaidSum = receipt.TryGetProperty("prepaidSum", out var ps) ? ps.GetDecimal() : (decimal?)null;
        var totalSum = receipt.TryGetProperty("totalSum", out var ts) ? ts.GetDecimal() : (decimal?)null;
        if (prepaidSum != null && totalSum != null && prepaidSum >= totalSum)
            continue;

        if (!receipt.TryGetProperty("items", out var itemsEl))
            continue;

        var items = itemsEl.EnumerateArray().ToList();
        if (!items.Any(i => i.TryGetProperty("name", out var n) && n.GetString() is { Length: > 0 }))
            continue;

        var user = receipt.TryGetProperty("user", out var u) ? u.GetString() : null;
        var retailPlace = receipt.TryGetProperty("retailPlace", out var rp) ? rp.GetString() : null;
        var dateTime = receipt.TryGetProperty("dateTime", out var dt) ? dt.GetString() : null;

        var store = (user != null && storeMapping.TryGetValue(user, out var s1) ? s1 : null)
            ?? (retailPlace != null && storeMapping.TryGetValue(retailPlace, out var s2) ? s2 : null)
            ?? user ?? retailPlace ?? "Unknown";

        foreach (var item in items)
        {
            var name = item.TryGetProperty("name", out var nm) ? nm.GetString() : null;
            if (string.IsNullOrEmpty(name))
                continue;

            records.Add(new
            {
                name,
                sum = item.TryGetProperty("sum", out var sm) ? sm.GetDecimal() : 0,
                quantity = item.TryGetProperty("quantity", out var qt) ? qt.GetDouble() : 0,
                price = item.TryGetProperty("price", out var pr) ? pr.GetDecimal() : 0,
                date = dateTime,
                store
            });
        }
    }

    var jsonOptions = new JsonSerializerOptions
    {
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

        foreach (var receipt in listResult.Receipts)
        {
            var fileId = $"{receipt.FiscalDriveNumber}_{receipt.FiscalDocumentNumber}";
            var filePath = Path.Combine(OutputDir, $"{fileId}.json");

            if (File.Exists(filePath))
            {
                skipped++;
                Console.WriteLine($"  SKIP {fileId} (already exists)");
                continue;
            }

            var detailBody = new { key = receipt.Key };
            var detailResponse = await http.PostAsJsonAsync($"{BaseUrl}/receipt/fiscal_data", detailBody);

            if (detailResponse.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                skipped++;
                Console.WriteLine($"  SKIP {fileId} (422 Unprocessable Entity)");
                continue;
            }

            detailResponse.EnsureSuccessStatusCode();

            var detailJson = await detailResponse.Content.ReadAsStringAsync();

            // Re-serialize with indentation for consistent formatting
            var detailDoc = JsonSerializer.Deserialize<JsonElement>(detailJson);
            var formatted = JsonSerializer.Serialize(detailDoc, jsonOptions);

            await File.WriteAllTextAsync(filePath, formatted, Encoding.UTF8);
            downloaded++;
            Console.WriteLine($"  OK   {fileId} — {receipt.TotalSum} — {receipt.KktOwner}");
        }

        if (listResult.HasMore != true)
            break;

        offset += PageSize;
    }

    Console.WriteLine($"\nDone. Downloaded: {downloaded}, Skipped: {skipped}");
    return 0;
}

record ReceiptListResponse(
    [property: JsonPropertyName("receipts")] List<ReceiptItem> Receipts,
    [property: JsonPropertyName("hasMore")] bool? HasMore
);

record ReceiptItem(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("fiscalDocumentNumber")] string FiscalDocumentNumber,
    [property: JsonPropertyName("fiscalDriveNumber")] string FiscalDriveNumber,
    [property: JsonPropertyName("totalSum")] string TotalSum,
    [property: JsonPropertyName("kktOwner")] string KktOwner,
    [property: JsonPropertyName("createdDate")] string CreatedDate
);
