namespace ReceiptDownloader;

class ReceiptSync(NalogClient client, ReceiptsRepository repo)
{
    const int PageSize = 10;
    const int MaxParallelism = 3;
    const int MaxRetries = 3;

    public async Task<(int Downloaded, int Skipped)> DownloadAll(CancellationToken ct = default)
    {
        int offset = 0;
        int downloaded = 0;
        int skipped = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var listResult = await WithRetry(innerCt => client.ListReceipts(PageSize, offset, innerCt), ct);
            if (listResult?.Receipts == null || listResult.Receipts.Count == 0)
                break;

            await Parallel.ForEachAsync(listResult.Receipts,
                new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism, CancellationToken = ct },
                async (receipt, itemCt) =>
            {
                var fileId = $"{receipt.FiscalDriveNumber}_{receipt.FiscalDocumentNumber}";

                if (repo.Exists(fileId))
                {
                    Interlocked.Increment(ref skipped);
                    Console.WriteLine($"  SKIP {fileId} (already exists)");
                    return;
                }

                var detail = await WithRetry(retryCt => client.GetReceiptDetail(receipt.Key, retryCt), itemCt);

                if (detail == null)
                {
                    Interlocked.Increment(ref skipped);
                    Console.WriteLine($"  SKIP {fileId} (422 Unprocessable Entity)");
                    return;
                }

                await repo.Save(fileId, detail, itemCt);
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
