using Microsoft.Extensions.Configuration;
using RestaurantNotifier.DataAccess;
using RestaurantNotifier.Model;
using RestaurantNotifier.Service;

namespace RestaurantNotifier;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"Hello world");
        // Build the configuration
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory!)
            .AddJsonFile("appsettings.json")
            .Build();

        // Access values from appsettings.json
        string apiKey = config["AppSettings:GooglePlacesApiKey"];
        string googlePlacesBaseUrl = config["AppSettings:GooglePlacesBaseUrl"];
        string connectionString = config["AppSettings:ConnectionString"];

        var api = new GooglePlacesApiWrapper(googlePlacesBaseUrl, apiKey);
        var db = new DbRepository(connectionString);

        var place = new Place()
        {
            place_id = "123",
            name = "test",
            rating = 1,
            user_ratings_total = 1
        };
        
        db.InsertRestaurant(place);
        
        var places = api.GetPlaces();
    }
}