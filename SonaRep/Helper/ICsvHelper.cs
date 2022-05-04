namespace SonaRep.Helper;

public interface ICsvHelper
{
    public void SaveToCsv<T>(List<T> data, string path);
}