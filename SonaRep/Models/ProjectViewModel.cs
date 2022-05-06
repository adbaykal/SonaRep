using SonaRep.Services.Models;

namespace SonaRep.Models;

public class ProjectViewModel 
{
    public string id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string qualifier { get; set; }
    public MeasureViewModel? reliabilityRating { get; set; }

    public MeasureViewModel? maintainabilityRating { get; set; }

    public MeasureViewModel? securityRating { get; set; }

    public List<MeasureViewModel> measures { get; set; }
}

public class MeasureViewModel : Measure
{
    public MeasureViewModel(Measure measure)
    {
        this.metric = measure.metric;
        this.value = measure.value;
        this.bestValue = measure.bestValue;
    }
    
    public string type { get; set; }
    public string name { get; set; }
    public string description { get; set; }

    public string formattedValue { get; set; }

    public string badgeClass { get; set; }
}

public class MetricModel
{
    public string value { get; set; }

    public string badgeClass { get; set; }
}