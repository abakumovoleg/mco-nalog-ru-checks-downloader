using ReceiptDownloader;


const string OutputFile = "data.js";
const string StoreMappingFile = "store_mapping.json";

var repo = new ReceiptsRepository();

if (args.Length > 0 && args[0] == "build")
    return DataBuilder.Build(repo, OutputFile, StoreMappingFile);

var token = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("NALOG_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.Error.WriteLine("Usage: ReceiptDownloader <bearer_token>  — download receipts");
    Console.Error.WriteLine("       ReceiptDownloader build           — build data.js from receipts");
    Console.Error.WriteLine("  or set NALOG_TOKEN environment variable");
    return 1;
}

Console.WriteLine("Fetching receipts...");

using var client = new NalogClient(token);
var sync = new ReceiptSync(client, repo);
var (downloaded, skipped) = await sync.DownloadAll();

Console.WriteLine($"\nDone. Downloaded: {downloaded}, Skipped: {skipped}");
return 0;
