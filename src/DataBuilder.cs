using System.Text;
using System.Text.Json;

namespace ReceiptDownloader;

static class DataBuilder
{
    public static int Build(ReceiptsRepository repo, string baseDir)
    {
        var outputFile = Path.Combine(baseDir, "data.js");
        var storeMappingFile = Path.Combine(baseDir, "store_mapping.json");

        var storeMapping = File.Exists(storeMappingFile)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(storeMappingFile)) ?? []
            : [];

        var records = new List<ReceiptRecord>();

        var fileCount = 0;

        foreach (var receipt in repo.ReadAll())
        {
            fileCount++;

            if (receipt.PrepaidSum != null && receipt.TotalSum != null && receipt.PrepaidSum >= receipt.TotalSum)
                continue;

            if (receipt.Items == null || receipt.Items.All(i => string.IsNullOrEmpty(i.Name)))
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

        var json = JsonSerializer.Serialize(records, JsonDefaults.Options);
        File.WriteAllText(outputFile, $"const RECEIPT_DATA = {json};\n", Encoding.UTF8);

        Console.WriteLine($"Processed {fileCount} files, extracted {records.Count} item records.");
        return 0;
    }
}
