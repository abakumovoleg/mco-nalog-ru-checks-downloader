using System.Text;
using System.Text.Json;

namespace ReceiptDownloader;

class ReceiptSync(NalogClient client, JsonSerializerOptions jsonOptions)
{
    const int PageSize = 10;
    const int MaxParallelism = 3;
    const int MaxRetries = 3;

    public async Task<(int Downloaded, int Skipped)> DownloadAll(string outputDir)
    {
        Directory.CreateDirectory(outputDir);

        int offset = 0;
        int downloaded = 0;
        int skipped = 0;

        while (true)
        {
            var listResult = await WithRetry(ct => client.ListReceipts(PageSize, offset, ct));
            if (listResult?.Receipts == null || listResult.Receipts.Count == 0)
                break;

            await Parallel.ForEachAsync(listResult.Receipts,
                new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism },
                async (receipt, ct) =>
            {
                var fileId = $"{receipt.FiscalDriveNumber}_{receipt.FiscalDocumentNumber}";
                var filePath = Path.Combine(outputDir, $"{fileId}.json");

                if (File.Exists(filePath))
                {
                    Interlocked.Increment(ref skipped);
                    Console.WriteLine($"  SKIP {fileId} (already exists)");
                    return;
                }

                var detail = await WithRetry(ct2 => client.GetReceiptDetail(receipt.Key, ct2), ct);

                if (detail == null)
                {
                    Interlocked.Increment(ref skipped);
                    Console.WriteLine($"  SKIP {fileId} (422 Unprocessable Entity)");
                    return;
                }

                var formatted = JsonSerializer.Serialize(detail, jsonOptions);
                await File.WriteAllTextAsync(filePath, formatted, Encoding.UTF8, ct);
                Interlocked.Increment(ref downloaded);
                Console.WriteLine($"  OK   {fileId} — {receipt.TotalSum} — {receipt.KktOwner}");
            });

            if (listResult.HasMore != true)
                break;

            offset += PageSize;
        }

        return (downloaded, skipped);
    }

    static async Task<T?> WithRetry<T>(Func<CancellationToken, Task<T?>> action, CancellationToken ct = default)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action(ct);
            }
            catch (HttpRequestException ex) when (attempt < MaxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                Console.WriteLine($"  RETRY (attempt {attempt}/{MaxRetries}: {ex.Message}), waiting {delay.TotalSeconds}s...");
                await Task.Delay(delay, ct);
            }
        }
    }
}
