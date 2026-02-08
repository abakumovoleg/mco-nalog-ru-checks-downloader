using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReceiptDownloader;

record ReceiptListRequest(
    int Limit,
    int Offset,
    string? DateFrom = null,
    string? DateTo = null,
    string OrderBy = "CREATED_DATE:DESC",
    string? Inn = null,
    string KktOwner = ""
);

record ReceiptListResponse(
    [property: JsonPropertyName("receipts")] List<ReceiptListItem> Receipts,
    [property: JsonPropertyName("hasMore")] bool? HasMore
);

record ReceiptListItem(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("fiscalDocumentNumber")] string FiscalDocumentNumber,
    [property: JsonPropertyName("fiscalDriveNumber")] string FiscalDriveNumber,
    [property: JsonPropertyName("totalSum")] string TotalSum,
    [property: JsonPropertyName("kktOwner")] string KktOwner,
    [property: JsonPropertyName("createdDate")] string CreatedDate
);

record Receipt
{
    [JsonPropertyName("user")] public string? User { get; init; }
    [JsonPropertyName("retailPlace")] public string? RetailPlace { get; init; }
    [JsonPropertyName("dateTime")] public string? DateTime { get; init; }
    [JsonPropertyName("totalSum")] public decimal? TotalSum { get; init; }
    [JsonPropertyName("prepaidSum")] public decimal? PrepaidSum { get; init; }
    [JsonPropertyName("items")] public List<ReceiptItem>? Items { get; init; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
}

record ReceiptItem
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("sum")] public decimal Sum { get; init; }
    [JsonPropertyName("quantity")] public double Quantity { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
}

record ReceiptRecord(string Name, decimal Sum, double Quantity, decimal Price, string? Date, string Store);
