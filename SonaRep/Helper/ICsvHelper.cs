using System.Data;

namespace SonaRep.Helper;

public interface ICsvHelper
{
    public void SaveToCsv<T>(List<T> data, string path);

    public void SaveToCsv(DataTable table, string path);
}