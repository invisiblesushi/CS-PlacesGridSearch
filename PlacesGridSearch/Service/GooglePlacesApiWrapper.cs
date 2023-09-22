using System.Globalization;
using Newtonsoft.Json;
using PlacesGridSearch.Model;

namespace PlacesGridSearch.Service;

public class GooglePlacesApiWrapper
{
    private static HttpClient? _httpClient;
    private readonly Uri _apiUrlBase;
    private string _apiKey;
    private CultureInfo _cultureInfo;
    private long _requestNumber;

    public GooglePlacesApiWrapper(string url, string apiKey)
    {
        _apiUrlBase = new Uri(url);
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _cultureInfo = new CultureInfo("en-US");
        _requestNumber = 0;
    }

    public List<Place> GetPlaces(double lat, double lon, int radius)
    {
        var places = new List<Place>();
        
        var url = $@"{_apiUrlBase}nearbysearch/json?location={lat.ToString(_cultureInfo)},{lon.ToString(_cultureInfo)}&radius={radius}&type=restaurant&key={_apiKey}";

        var json = FetchResult(url).Result;

        var result = JsonConvert.DeserializeObject<GooglePlacesResult>(json);
        places.AddRange(result?.results!);

        while (result!.next_page_token != null)
        {
            var oldToken = result.next_page_token;
            Thread.Sleep(100);
            
            var nextPageUrl = $"{_apiUrlBase}nearbysearch/json?pagetoken={result.next_page_token}&key={_apiKey}";
            json = FetchResult(nextPageUrl).Result;
            result = JsonConvert.DeserializeObject<GooglePlacesResult>(json);

            // Force retry on invalid request.
            if (result?.status == "INVALID_REQUEST")
            {
                result.next_page_token = oldToken;
                Thread.Sleep(1000);
            }
            
            places.AddRange(result?.results!);
        }

        Console.WriteLine($"Request returned {places.Count} results");
        if (places.Count == 60)
            Console.WriteLine("60 results pr search limit reached!");

        return places;
    }

    private async Task<string> FetchResult(string url)
    {
        var response = await _httpClient!.GetAsync(url);
        
        _requestNumber += 1;
        Console.WriteLine($"Request number: {_requestNumber}");
        
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Request failed");

        return await response.Content.ReadAsStringAsync();
    }
}