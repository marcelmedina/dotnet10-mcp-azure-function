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
            propertyType: "string",
            description: "The name of the restaurant.")]
        string name,
        [McpToolProperty(
            propertyName: "location",
            propertyType: "string",
            description: "The location/address of the restaurant.")]
        string location,
        [McpToolProperty(
            propertyName: "foodType",
            propertyType: "string",
            description: "The type of food served (e.g., Italian, Mexican, Thai, etc.)")]
        string foodType)
    {
        var restaurant = await restaurantService.AddRestaurantAsync(name, location, foodType);
        return restaurant;
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