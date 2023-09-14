using System.Globalization;
using CoordinateSharp;
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

        var places = new List<Place>();
        
        //var places = api.GetPlaces();
        //db.InsertRestaurant(places);
        CultureInfo culture = new CultureInfo("en-US");

        
        // Start location at top left corner
        // Gulleråsen subway station
        double startLat = 59.956104;
        double startLon = 10.696484;

        Coordinate coordinate = new Coordinate(startLat, startLon);
        Console.WriteLine($"https://www.google.com/maps?q={coordinate.Latitude.ToDouble().ToString(culture)},{coordinate.Longitude.ToDouble().ToString(culture)}");
        
        // Grid size in meters
        double xGrid = 8000;
        double yGrid = 8000;

        // Distance to move and radius of search query.
        int moveDistance = 200;

        // Grid start at top left corner and moved down and right.
        double xCurr = 0;
        double yCurr = 0;
        Console.WriteLine($"x = {xCurr} y = {yCurr}");
        
        for (double j = 0; j < yGrid; j += moveDistance)
        {
            // Move right x+3.0
            for (var i = 0; i < xGrid; i += moveDistance)
            {
                xCurr++;
                coordinate.Move(moveDistance, 90, Shape.Ellipsoid);
                places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
            }

            // Move left x-0.5 down y-0.5
            yCurr -= 0.5;
            xCurr -= 0.5;
            coordinate.Move(moveDistance/2, 225, Shape.Ellipsoid);
            places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));

            // Move left x-3.0
            for (int i = 0; i < xGrid; i += moveDistance)
            {
                xCurr -= 1;
                coordinate.Move(moveDistance, 270, Shape.Ellipsoid);
                places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
            }
        
            // Move right x+0.5 down y-0.5
            yCurr -= 0.5;
            xCurr += 0.5;
            coordinate.Move(moveDistance/2, 135, Shape.Ellipsoid); 
            places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
            
            Console.WriteLine($"Places.Count {places.Count}");
        }

        Console.WriteLine($"Places.Count Before {places.Count}");
        places = places.GroupBy(p => p.place_id).Select(group => @group.First()).ToList();
        Console.WriteLine($"Places.Count After {places.Count}");
        db.InsertRestaurant(places);

    }
    
 
}