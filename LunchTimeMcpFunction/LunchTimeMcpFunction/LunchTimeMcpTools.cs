using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using LunchTimeMCP; // RestaurantService + Restaurant models

namespace LunchTimeMcpFunction;

public class LunchTimeMcpTools
{
    private readonly RestaurantService restaurantService;

    public LunchTimeMcpTools(RestaurantService restaurantService)
    {
        this.restaurantService = restaurantService;
    }

    [Function(nameof(GetRestaurants))]
    public async Task<object> GetRestaurants(
        [McpToolTrigger(
            toolName: "get_restaurants",
            description: "Get a list of all restaurants available for lunch.")]
        ToolInvocationContext context)
    {
        var restaurants = await restaurantService.GetRestaurantsAsync();
        return restaurants;
    }

    [Function(nameof(AddRestaurant))]
    public async Task<Restaurant> AddRestaurant(
        [McpToolTrigger(
            toolName: "add_restaurant",
            description: "Add a new restaurant to the lunch options.")]
        ToolInvocationContext context,
        [McpToolProperty(
            propertyName: "name",
            description: "The name of the restaurant.",
            IsRequired = true)]
        string name,
        [McpToolProperty(
            propertyName: "location",
            description: "The location/address of the restaurant.",
            IsRequired = true)]
        string location,
        [McpToolProperty(
            propertyName: "foodType",
            description: "The type of food served (e.g., Italian, Mexican, Thai, etc.)",
            IsRequired = true)]
        string foodType)
    {
        var restaurant = await restaurantService.AddRestaurantAsync(name, location, foodType);
        return restaurant;
    }

    [Function(nameof(DeleteRestaurant))]
    public async Task<object> DeleteRestaurant(
        [McpToolTrigger(
            toolName: "delete_restaurant",
            description: "Delete a restaurant from the lunch options by its ID.")]
        ToolInvocationContext context,
        [McpToolProperty(
            propertyName: "id",
            description: "The ID of the restaurant to delete.",
            IsRequired = true)]
        string id)
    {
        var deleted = await restaurantService.DeleteRestaurantAsync(id);

        if (!deleted)
        {
            return new { message = $"Restaurant with ID '{id}' was not found." };
        }

        return new { message = $"Restaurant with ID '{id}' has been deleted." };
    }

    [Function(nameof(PickRandomRestaurant))]
    public async Task<object> PickRandomRestaurant(
        [McpToolTrigger(
            toolName: "pick_random_restaurant",
            description: "Pick a random restaurant from the available options for lunch.")]
        ToolInvocationContext context)
    {
        var selected = await restaurantService.PickRandomRestaurantAsync();

        if (selected is null)
        {
            return new
            {
                message = "No restaurants available. Please add some restaurants first!"
            };
        }

        return new
        {
            message = "Randomly selected restaurant for lunch.",
            restaurant = selected
        };
    }
}