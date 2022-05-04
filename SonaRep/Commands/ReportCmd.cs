using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using SonaRep.Models;
using SonaRep.Services;
using SonaRep.Services.Models;

namespace SonaRep.Commands;

[Command(Name = "report", Description = "Generates a report based on given options. Use --help to see options.")]
public class ReportCmd : CommandBase
{
    [Required]
    [AllowedValues("fav","single")]
    [Option(CommandOptionType.SingleValue, LongName = "repotype",ShortName = "rt", Description = "Type of Repositories",  ShowInHelpText = true)]  
    public string RepoType { get; set; }

    [AllowedValues("csv","json","pdf")]
    [Option(CommandOptionType.SingleValue, LongName = "outputtype",ShortName = "o",Description = "Type of output.",
         ShowInHelpText = true)]
    public string OutputType { get; set; } = "csv";
    
    [Option(CommandOptionType.SingleValue, LongName = "repo",ShortName = "r",Description = "Repo id for single repo reporting. Only works with single repotype.",
        ShowInHelpText = true)]
    public string? RepoId { get; set; }
    
    private readonly UserProfileModel _userProfileModel;
    private readonly ISonarService _sonarService;
    
    public ReportCmd(
        IConsole console,
        UserProfileModel userProfileModel,
        ISonarService sonarService)
    {
        _console = console;
        _userProfileModel = userProfileModel;
        _sonarService = sonarService;
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
                var favorites = await PopulateFavorites();
                if (favorites == null)
                    return -1;
                break;
            }
            case "single" when string.IsNullOrEmpty(RepoId):
                _console.WriteLine("You need to specify RepoName for single repotype.");
                return -1;
            case "single":
            {
                var component = await GetSingleRepoMetrics(RepoId);
                if (component == null)
                    return -1;
                break;
            }
        }
        
        //TODO: export report
        switch (OutputType)
        {
            case "csv":
                break;
            case "json" :
                break;
            case "pdf":
                break;
        }
        
        return 0;
    }

    private async Task<List<Component>?> PopulateFavorites()
    {
        var favList = await _sonarService.GetFavoritesAsync(_userProfileModel.Token);

        if (favList == null)
        {
            _console.WriteLine("No favorite repo is found on sonarcloud.");
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