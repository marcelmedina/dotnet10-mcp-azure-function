using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunchTimeMCP;

public class RestaurantService
{
    private readonly TableClient tableClient;
    private const string TableName = "restaurants";

    public RestaurantService(TableServiceClient tableServiceClient)
    {
        tableClient = tableServiceClient.GetTableClient(TableName);
        tableClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
    }

    public async Task<List<Restaurant>> GetRestaurantsAsync()
    {
        var restaurants = new List<Restaurant>();

        await foreach (var entity in tableClient.QueryAsync<RestaurantEntity>())
        {
            restaurants.Add(new Restaurant
            {
                Id = entity.RowKey,
                Name = entity.Name,
                Location = entity.Location,
                FoodType = entity.FoodType,
                VisitCount = entity.VisitCount,
                LastVisited = entity.LastVisited
            });
        }

        return restaurants;
    }

    public async Task<Restaurant> AddRestaurantAsync(string name, string location, string foodType)
    {
        var id = Guid.NewGuid().ToString();
        var entity = new RestaurantEntity
        {
            RowKey = id,
            Name = name,
            Location = location,
            FoodType = foodType,
            VisitCount = 0
        };

        await tableClient.AddEntityAsync(entity);

        return new Restaurant
        {
            Id = id,
            Name = name,
            Location = location,
            FoodType = foodType,
            VisitCount = 0
        };
    }

    public async Task<Restaurant?> PickRandomRestaurantAsync()
    {
        var restaurants = await GetRestaurantsAsync();

        if (!restaurants.Any())
        {
            return null;
        }

        var random = new Random();
        var selected = restaurants[random.Next(restaurants.Count)];

        // Update visit count and last visited timestamp
        var entity = await tableClient.GetEntityAsync<RestaurantEntity>("restaurants", selected.Id);
        entity.Value.VisitCount++;
        entity.Value.LastVisited = DateTime.UtcNow;
        await tableClient.UpdateEntityAsync(entity.Value, entity.Value.ETag);

        selected.VisitCount = entity.Value.VisitCount;
        selected.LastVisited = entity.Value.LastVisited;

        return selected;
    }
}

public class Restaurant
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty;
    public int VisitCount { get; set; }
    public DateTime? LastVisited { get; set; }
}