using System.Text;
using System.Text.Json;

namespace ReceiptDownloader;

class ReceiptsRepository
{
    readonly string _outputDir;

    public ReceiptsRepository(string baseDir)
    {
        _outputDir = Path.Combine(baseDir, "receipts");
        Directory.CreateDirectory(_outputDir);
    }

    public bool Exists(string fileId) => File.Exists(GetPath(fileId));

    public async Task Save(string fileId, Receipt receipt, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(receipt, JsonDefaults.Options);
        await File.WriteAllTextAsync(GetPath(fileId), json, Encoding.UTF8, ct);
    }

    public IEnumerable<Receipt> ReadAll()
    {
        foreach (var file in Directory.GetFiles(_outputDir, "*.json"))
        {
            var raw = File.ReadAllText(file, Encoding.UTF8).TrimStart('\uFEFF');
            var receipt = JsonSerializer.Deserialize<Receipt>(raw, JsonDefaults.Options);
            if (receipt != null)
                yield return receipt;
        }
    }

    string GetPath(string fileId) => Path.Combine(_outputDir, $"{fileId}.json");
}
