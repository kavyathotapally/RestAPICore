namespace RestAPICore.Models;

public class CachedCityDetails
{
    public string city_id { get; set; }
    public string name { get; set; }
    public string state { get; set; }
    public string lat_long { get; set; }
    public int? is_package { get; set; }
}