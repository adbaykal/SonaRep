using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace SonaRep.Commands;

[Command(Name = "sonarep", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase )]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[Subcommand(
    typeof(LoginCmd),
    typeof(ReportCmd)
    )]
public class SonarepCmd : CommandBase
{
    public SonarepCmd(IConsole? console)
    {
        _console = console;
    }
    
    protected override Task<int> OnExecute(CommandLineApplication app)
    {
        // this shows help even if the --help option isn't specified
        app.ShowHelp();            
        return Task.FromResult(0);
    }
    private static string GetVersion()
        => typeof(SonarepCmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
}