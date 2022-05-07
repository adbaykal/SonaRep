using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using McMaster.Extensions.CommandLineUtils;
using SonaRep.Helper;
using SonaRep.Models;
using SonaRep.Services;
using SonaRep.Services.Models;
using MetricModel = SonaRep.Services.Models.MetricModel;

namespace SonaRep.Commands;

[Command(Name = "report", Description = "Generates a report based on given options. Use --help to see options.")]
public class ReportCmd : CommandBase
{
    [Required]
    [AllowedValues("fav","single")]
    [Option(CommandOptionType.SingleValue, LongName = "repotype",ShortName = "rt", Description = "Type of Repositories",  ShowInHelpText = true)]  
    public string RepoType { get; set; }

    [AllowedValues("csv","json","html","png","pdf")]
    [Option(CommandOptionType.SingleValue, LongName = "outputtype",ShortName = "o",Description = "Type of output.",
         ShowInHelpText = true)]
    public string OutputType { get; set; } = "csv";
    
    [Option(CommandOptionType.SingleValue, LongName = "repo",ShortName = "r",Description = "Repo id for single repo reporting. Only works with single repotype.",
        ShowInHelpText = true)]
    public string? RepoId { get; set; }

    [Option(CommandOptionType.SingleValue, LongName = "path", ShortName = "p", Description = "Path of the report file.",
        ShowInHelpText = true)]
    public string Path { get; set; } = "";
    
    [Option(CommandOptionType.SingleValue, LongName = "filename", ShortName = "f", Description = "Name of the report file.",
        ShowInHelpText = true)]
    public string FileName { get; set; } = "SonarepReport";
    
    private readonly UserProfileModel _userProfileModel;
    private readonly ISonarService _sonarService;
    private readonly IReportExportService _exportService;
    
    public ReportCmd(
        IConsole? console,
        UserProfileModel userProfileModel,
        ISonarService sonarService,
        IReportExportService exportService)
    {
        _console = console;
        _userProfileModel = userProfileModel;
        _sonarService = sonarService;
        _exportService = exportService;
    }
    
    protected override async Task<int> OnExecute(CommandLineApplication app)
    {
        if (string.IsNullOrEmpty(_userProfileModel.Token)){
            _console.WriteLine("Token not found. Please run login command before generating a report.");
            return -1;
        }

        var projectList = new List<Component>();
        switch (RepoType)
        {
            case "fav":
            {
                _console.WriteLine("Fetching favorite projects from SonarCloud.");
                var favorites = await PopulateFavorites();
                if (favorites == null)
                    return -1;
                projectList = favorites;
                break;
            }
            case "single" when string.IsNullOrEmpty(RepoId):
                _console.WriteLine("You need to specify RepoId for single repotype.");
                return -1;
            case "single":
            {
                _console.WriteLine("Fetching the project from SonarCloud. RepoId: " + RepoId);
                var component = await GetSingleRepoMetrics(RepoId);
                if (component == null)
                    return -1;
                projectList.Add(component);
                break;
            }
        }
        
        _console.WriteLine("Exporting report. OutputType: " + OutputType);
        var fullPath = "";
        MetricModel? metricDefs;
        switch (OutputType)
        {
            case "csv":
                fullPath = _exportService.ExportAsCsv(projectList, System.IO.Path.Combine(Path,FileName + ".csv"));
                _console.WriteLine($"Report created successfully! Path: {fullPath}");
                break;
            case "json" :
                fullPath = _exportService.ExportAsJson(projectList, System.IO.Path.Combine(Path,FileName + ".json"));
                _console.WriteLine($"Report created successfully! Path: {fullPath}");
                break;
            case "html":
                metricDefs = await _sonarService.ListMetricsAsync(_userProfileModel.Token);
                fullPath = _exportService.ExportAsHtml(projectList, System.IO.Path.Combine(Path,FileName + ".html"),metricDefs);
                _console.WriteLine($"Report created successfully! Path: {fullPath}");
                break;
            case "png":
                metricDefs = await _sonarService.ListMetricsAsync(_userProfileModel.Token);
                fullPath = await _exportService.ExportAsPng(projectList, System.IO.Path.Combine(Path,FileName + ".png"),metricDefs);
                _console.WriteLine($"Report created successfully! Path: {fullPath}");
                break;
            case "pdf":
                metricDefs = await _sonarService.ListMetricsAsync(_userProfileModel.Token);
                fullPath = await _exportService.ExportAsPdf(projectList, System.IO.Path.Combine(Path,FileName + ".pdf"),metricDefs);
                _console.WriteLine($"Report created successfully! Path: {fullPath}");
                break;
        }
        
        return 0;
    }

    private async Task<List<Component>?> PopulateFavorites()
    {
        var favList = await _sonarService.GetFavoritesAsync(_userProfileModel.Token);

        if (favList == null)
        {
            _console.WriteLine("No favorite repo is found on SonarCloud.");
            return null;
        }

        var projectList = new List<Component>();
        foreach (var project in favList)
        {
            var component = await GetSingleRepoMetrics(project.key);
            if (component == null) continue;
            
            projectList.Add(component);
        }

        return projectList;
    }

    private async Task<Component?> GetSingleRepoMetrics(string repoId)
    {
        var component = await _sonarService.GetProjectDetailsAsync(_userProfileModel.Token, repoId);

        if (component == null)
        {
            _console.WriteLine($"Cannot get measures for project: {repoId}");
        }
        
        return component;
    }
}