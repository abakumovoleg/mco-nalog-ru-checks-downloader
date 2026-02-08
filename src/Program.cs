using ReceiptDownloader;

var baseDir = Path.GetFullPath("..");
var repo = new ReceiptsRepository(baseDir);

if (args.Length > 0 && args[0] == "build")
    return DataBuilder.Build(repo, baseDir);

var token = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("NALOG_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.Error.WriteLine("Usage: ReceiptDownloader <bearer_token>  — download receipts");
    Console.Error.WriteLine("       ReceiptDownloader build           — build data.js from receipts");
    Console.Error.WriteLine("  or set NALOG_TOKEN environment variable");
    return 1;
}

Console.WriteLine("Fetching receipts...");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

using var client = new NalogClient(token);
var sync = new ReceiptSync(client, repo);

try
{
    var (downloaded, skipped) = await sync.DownloadAll(cts.Token);
    Console.WriteLine($"\nDone. Downloaded: {downloaded}, Skipped: {skipped}");
    return 0;
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nCancelled.");
    return 1;
}
