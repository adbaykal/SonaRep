using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SonaRep.Models;

public class UserProfileModel
{
    private string token;
    public string Token {
        get
        {
            if (!string.IsNullOrEmpty(token)) return token;
            var text = File.ReadAllText(ProfileFilePath);
            if (!string.IsNullOrEmpty(text))
            {
                dynamic profile = JsonNode.Parse(text);
                if (profile is not null)
                {
                    this.token = profile["Token"].ToString();
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