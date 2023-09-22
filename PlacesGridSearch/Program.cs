using System.Globalization;
using CoordinateSharp;
using Microsoft.Extensions.Configuration;
using PlacesGridSearch.DataAccess;
using PlacesGridSearch.Model;
using PlacesGridSearch.Service;

namespace PlacesGridSearch;

public class Program
{
    private static CultureInfo _culture = new("en-US");

    public static void Main(string[] args)
    {
        // Build the configuration
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory!)
            .AddJsonFile("appsettings.json")
            .Build();

        // Access values from appsettings.json
        var apiKey = config["AppSettings:GooglePlacesApiKey"];
        var googlePlacesBaseUrl = config["AppSettings:GooglePlacesBaseUrl"];
        var connectionString = config["AppSettings:ConnectionString"];

        var api = new GooglePlacesApiWrapper(googlePlacesBaseUrl, apiKey);
        var db = new DbRepository(connectionString);

        var places = new List<Place>();
        
        // Start location at top left corner
        var startLat = 59.956104;
        var startLon = 10.696484;
        var coordinate = new Coordinate(startLat, startLon);

        // Grid size in meters
        double xGrid = 8000;
        double yGrid = 8000;
        
        // Distance to move and radius of search query.
        var moveDistance = 500;

        Console.WriteLine($"https://www.google.com/maps?q={coordinate.Latitude.ToDouble().ToString(_culture)},{coordinate.Longitude.ToDouble().ToString(_culture)}");

        // Grid start at top left corner and moved down and right.
        double xCurr = 0;
        double yCurr = 0;
        
        for (double j = 0; j < yGrid; j += moveDistance)
        {
            // Move right x+3.0
            for (var i = 0; i < xGrid; i += moveDistance)
            {
                xCurr++;
                coordinate.Move(moveDistance, 90, Shape.Ellipsoid);
                places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
                CoordinateConsoleOutput(xCurr, yCurr, coordinate);
            }

            // Move left x-0.5 down y-0.5
            yCurr -= 0.5;
            xCurr -= 0.5;
            coordinate.Move(moveDistance/2, 225, Shape.Ellipsoid);
            places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
            CoordinateConsoleOutput(xCurr, yCurr, coordinate);

            // Move left x-3.0
            for (int i = 0; i < xGrid; i += moveDistance)
            {
                xCurr -= 1;
                coordinate.Move(moveDistance, 270, Shape.Ellipsoid);
                places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
                CoordinateConsoleOutput(xCurr, yCurr, coordinate);
            }
        
            // Move right x+0.5 down y-0.5
            yCurr -= 0.5;
            xCurr += 0.5;
            coordinate.Move(moveDistance/2, 135, Shape.Ellipsoid);
            places.AddRange(api.GetPlaces(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), moveDistance));
            CoordinateConsoleOutput(xCurr, yCurr, coordinate);
        }

        // Remove duplicates.
        places = places.GroupBy(p => p.place_id).Select(group => @group.First()).ToList();
        
        // Insert to db
        db.InsertRestaurant(places);
    }

    private static void CoordinateConsoleOutput(double xGrix, double yGrid, Coordinate coordinate)
    {
        Console.WriteLine($"x = {xGrix} y = {yGrid}");
        Console.WriteLine($"https://www.google.com/maps?q={coordinate.Latitude.ToDouble().ToString(_culture)},{coordinate.Longitude.ToDouble().ToString(_culture)}");
    }
}