using System.Text;
using System.Text.Json;

namespace ReceiptDownloader;

static class DataBuilder
{
    public static int Build(string receiptsDir, string outputFile, string storeMappingFile, JsonSerializerOptions jsonOptions)
    {
        var storeMapping = File.Exists(storeMappingFile)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(storeMappingFile)) ?? []
            : [];

        var files = Directory.GetFiles(receiptsDir, "*.json");
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

        var json = JsonSerializer.Serialize(records, jsonOptions);
        File.WriteAllText(outputFile, $"const RECEIPT_DATA = {json};\n", Encoding.UTF8);

        Console.WriteLine($"Processed {files.Length} files, extracted {records.Count} item records.");
        return 0;
    }
}
