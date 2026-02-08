using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReceiptDownloader;

class ReceiptsRepository
{
    const string OutputDir = "../receipts";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ReceiptsRepository()
    {
        Directory.CreateDirectory(OutputDir);
    }

    public bool Exists(string fileId) => File.Exists(GetPath(fileId));

    public async Task Save(string fileId, Receipt receipt, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(receipt, _jsonOptions);
        await File.WriteAllTextAsync(GetPath(fileId), json, Encoding.UTF8, ct);
    }

    public IEnumerable<Receipt> ReadAll()
    {
        foreach (var file in Directory.GetFiles(OutputDir, "*.json"))
        {
            var raw = File.ReadAllText(file, Encoding.UTF8).TrimStart('\uFEFF');
            var receipt = JsonSerializer.Deserialize<Receipt>(raw);
            if (receipt != null)
                yield return receipt;
        }
    }

    string GetPath(string fileId) => Path.Combine(OutputDir, $"{fileId}.json");
}
