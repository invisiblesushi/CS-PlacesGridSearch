namespace RestaurantNotifier.Model;

public class Place
{
    public string business_status { get; set; }
    public Geometry geometry { get; set; }
    public string icon { get; set; }
    public string icon_background_color { get; set; }
    public string icon_mask_base_uri { get; set; }
    public string name { get; set; }
    public List<Photo> photos { get; set; }
    public string place_id { get; set; }
    public double rating { get; set; }
    public string reference { get; set; }
    public string scope { get; set; }
    public List<string> types { get; set; }
    public int user_ratings_total { get; set; }
    public string vicinity { get; set; }
    public int? price_level { get; set; }
}

public class Geometry
{
    public Location location { get; set; }
}

public class Location
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class Photo
{
    public int height { get; set; }
    public List<string> html_attributions { get; set; }
    public string photo_reference { get; set; }
    public int width { get; set; }
}