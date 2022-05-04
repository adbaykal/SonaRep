using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using SonaRep.Models;

namespace SonaRep.Commands;

[Command(Name = "login", Description = "Login to sonarcloud with given token, the token will be saved locally in the profile. Token needs to be specified with --token or -t option.")]
public class LoginCmd : CommandBase
{
    [Required]
    [Option(CommandOptionType.SingleValue, ShortName = "t", LongName = "token", Description = "sonarcloud login token", ValueName = "login token", ShowInHelpText = true)]       
    public string Token { get; set; }

    private UserProfileModel _userProfileModel;
    
    public LoginCmd(
        IConsole console,
        UserProfileModel userProfileModel)
    {
        _console = console;
        _userProfileModel = userProfileModel;
    }
    
    protected override Task<int> OnExecute(CommandLineApplication app)
    {
        _userProfileModel.Token = this.Token;
        _console.WriteLine($"Token saved to your profile file. Path: {_userProfileModel.ProfileFilePath}");
        
        return Task.FromResult(0);
    }
}