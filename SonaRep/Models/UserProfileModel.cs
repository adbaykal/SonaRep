using System.Text;
using System.Text.Json;

namespace SonaRep.Models;

public class UserProfileModel
{
    private string token;
    public string Token {
        get
        {
            if (!string.IsNullOrEmpty(token)) return token;
            var text = File.ReadAllText($"{ProfileFolder}profile.json");
            if (!string.IsNullOrEmpty(text))
            {
                var profile = JsonSerializer.Deserialize<UserProfileModel>(text);
                if (profile != null)
                {
                    this.Token = profile.Token;
                }
                    
            }

            return token;
        }
        set
        {
            token = value;
            if (!Directory.Exists(ProfileFolder))
            {
                Directory.CreateDirectory(ProfileFolder);
            }
            
            File.WriteAllText(ProfileFilePath, JsonSerializer.Serialize(this), Encoding.UTF8);
        } 
    }

    public string ProfileFilePath {
        get
        {
            var profileFilePath = Path.Combine(ProfileFolder, "sonarep.json");
            return profileFilePath;
        }
    }
    
    protected static string ProfileFolder
    {
        get
        {
             
            var profileFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create)}";
            profileFolder = Path.Combine(profileFolder, ".sonarep");
            return profileFolder;
        }
    }
}