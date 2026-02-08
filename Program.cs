using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReceiptDownloader;

const string OutputDir = "receipts";
const string OutputFile = "data.js";
const string StoreMappingFile = "store_mapping.json";

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

if (args.Length > 0 && args[0] == "build")
    return DataBuilder.Build(OutputDir, OutputFile, StoreMappingFile, jsonOptions);

var token = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("NALOG_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.Error.WriteLine("Usage: ReceiptDownloader <bearer_token>  — download receipts");
    Console.Error.WriteLine("       ReceiptDownloader build           — build data.js from receipts");
    Console.Error.WriteLine("  or set NALOG_TOKEN environment variable");
    return 1;
}

Console.WriteLine("Fetching receipts...");

using var client = new NalogClient(token, jsonOptions);
var sync = new ReceiptSync(client, jsonOptions);
var (downloaded, skipped) = await sync.DownloadAll(OutputDir);

Console.WriteLine($"\nDone. Downloaded: {downloaded}, Skipped: {skipped}");
return 0;
