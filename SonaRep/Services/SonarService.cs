using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SonaRep.Services.Models;
using MetricModel = SonaRep.Services.Models.MetricModel;

namespace SonaRep.Services;

public class SonarService : ISonarService
{
    private readonly ILogger<SonarService> _logger;
    private HttpClient client;

    private readonly string BaseUrl = "https://sonarcloud.io/api/";
    private static string defaultMetricKeys = "blocker_violations,code_smells,new_branch_coverage,new_coverage,coverage,critical_violations,duplicated_lines_density,effort_to_reach_maintainability_rating_a,new_maintainability_rating,major_violations,blocker_violations,security_hotspots,vulnerabilities,alert_status,reliability_rating,security_hotspots,security_rating,sqale_debt_ratio,sqale_rating";
    
    public SonarService(
        ILogger<SonarService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        client = httpClientFactory.CreateClient();
    }
    public async Task<List<Favorite>?> GetFavoritesAsync(string token)
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            BaseUrl + "favorites/search")
        {
            Headers =
            {
                {HeaderNames.Authorization, $"Basic {Base64Encode($"{token}:")}"},
                {HeaderNames.Accept, "application/json" }
            }
        };
     

        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Sonar Favorites List Service returned HttpResponse other than 200.");
            _logger.LogError("StatusCode: {statusCode} Body: {body}", response.StatusCode, response.Content.ReadAsStringAsync());
            return null;
        }
        else
        {
            var responseBodyStream = await response.Content.ReadAsStreamAsync();
            var favList = await JsonSerializer.DeserializeAsync<FavoriteListModel>(responseBodyStream);
            return favList.favorites;
        }
        
    }

    public async Task<Component?> GetProjectDetailsAsync(string token, string projectName)
    {
        var responseBodyStream = await GetComponentDetailsAsync(token, projectName, defaultMetricKeys);
        if (responseBodyStream == null) return null;
        var reader = new StreamReader(responseBodyStream);
        var responseBody = reader.ReadToEnd();
        _logger.LogTrace(responseBody);
        var project = JsonSerializer.Deserialize<ComponentDetailModel>(responseBody);
        return project?.component;
    }

    public async Task<string?> GetProjectDetailsAsync(string token, string projectName, string metricKeys)
    {
        var responseBodyStream = await GetComponentDetailsAsync(token, projectName, defaultMetricKeys);
        if (responseBodyStream == null) return null;
        StreamReader reader = new StreamReader(responseBodyStream);
        var project = await reader.ReadToEndAsync();
        return project;
    }

    public async Task<MetricModel?> ListMetricsAsync(string token)
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            BaseUrl + "metrics/search?ps=500")
        {
            Headers =
            {
                {HeaderNames.Authorization, $"Basic {Base64Encode($"{token}:")}"},
                {HeaderNames.Accept, "application/json" }
            }
        };
     

        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Sonar Favorites List Service returned HttpResponse other than 200.");
            _logger.LogError("StatusCode: {statusCode} Body: {body}", response.StatusCode, response.Content.ReadAsStringAsync());
            return null;
        }
        else
        {
            var responseBodyStream = await response.Content.ReadAsStreamAsync();
            var favList = await JsonSerializer.DeserializeAsync<MetricModel>(responseBodyStream);
            return favList;
        }
    }

    private async Task<Stream?> GetComponentDetailsAsync(string token, string projectName, string metricKeys)
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            BaseUrl + "measures/component?" + 
            "component=" + projectName +
            "&metricKeys=" + metricKeys)
        {
            Headers =
            {
                {HeaderNames.Authorization, $"Basic {Base64Encode($"{token}:")}"},
                {HeaderNames.Accept, "application/json" }
            }
        };

        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Sonar Project Details Service returned HttpResponse other than 200.");
            _logger.LogError("StatusCode: {statusCode} Body: {body}", response.StatusCode, response.Content.ReadAsStringAsync());
            return null;
        }
        else
        {
            var responseBodyStream = await response.Content.ReadAsStreamAsync();
            return responseBodyStream;
        }
    }
    
    private static string Base64Encode(string textToEncode)
    {
        byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
        return Convert.ToBase64String(textAsBytes);
    }
}