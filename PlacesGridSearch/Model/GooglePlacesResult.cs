namespace PlacesGridSearch.Model;

public class GooglePlacesResult
{
    public List<object> html_attributions { get; set; }
    public string next_page_token { get; set; }
    public List<Place> results { get; set; }
    public string status { get; set; }
}