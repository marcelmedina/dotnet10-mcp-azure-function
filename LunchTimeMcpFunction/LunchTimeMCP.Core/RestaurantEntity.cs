using Azure;
using Azure.Data.Tables;
using System;

namespace LunchTimeMCP;

public class RestaurantEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "restaurants";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty;
    public int VisitCount { get; set; } = 0;
    public DateTime? LastVisited { get; set; }
}