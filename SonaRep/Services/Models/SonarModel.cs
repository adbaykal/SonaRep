namespace SonaRep.Services.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Paging
{
    public int pageIndex { get; set; }
    public int pageSize { get; set; }
    public int total { get; set; }
}
public class PageableModel
{
    public Paging paging { get; set; }
}

public class Favorite
{
    public string organization { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string qualifier { get; set; }
}
public class Measure
{
    public string metric { get; set; }
    public double value { get; set; }
    public bool bestValue { get; set; }
}

public class Component
{
    public string id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string qualifier { get; set; }
    public List<Measure> measures { get; set; }
}

public class ComponentDetailModel
{
    public Component component { get; set; }
}

public class FavoriteListModel: PageableModel
{
    public List<Favorite> favorites { get; set; }
}
