using Newtonsoft.Json;
using RestaurantNotifier.Model;

namespace RestaurantNotifier.Service;

public class GooglePlacesApiWrapper
{
    private static HttpClient? _httpClient;
    private readonly Uri _apiUrlBase;
    private string _apiKey;

    public GooglePlacesApiWrapper(string url, string apiKey)
    {
        _apiUrlBase = new Uri(url);
        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    public List<Place> GetPlaces()
    {
        var places = new List<Place>();
        
        var url = $@"{_apiUrlBase}nearbysearch/json?location=59.9139,10.7522&radius=100&type=restaurant&key={_apiKey}";

        var json = FetchResult(url).Result;

        var result = JsonConvert.DeserializeObject<GooglePlacesResult>(json);
        places.AddRange(result.results);

        while (result.next_page_token != null)
        {
            var nextPageUrl = $"{_apiUrlBase}place/nearbysearch/json?pagetoken={result.next_page_token}&key={_apiKey}";
            json = FetchResult(url).Result;
            result = JsonConvert.DeserializeObject<GooglePlacesResult>(json);
            places.AddRange(result.results);
        }

        return places;
    }

    private async Task<string> FetchResult(string url)
    {
        HttpResponseMessage response = null;
        
        response = await _httpClient!.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Request failed");
        }
        
        return await response!.Content.ReadAsStringAsync();
    }
}