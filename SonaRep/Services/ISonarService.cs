using SonaRep.Services.Models;

namespace SonaRep.Services;

public interface ISonarService
{
    public Task<List<Favorite>?> GetFavoritesAsync(string token);

    public Task<Component?> GetProjectDetailsAsync(string token, string projectName);
    
    public Task<string?> GetProjectDetailsAsync(string token, string projectName, string metricKeys);
    
    public Task<MetricModel?> ListMetricsAsync(string token);
}
