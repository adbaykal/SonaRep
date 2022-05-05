using SonaRep.Services.Models;

namespace SonaRep.Services;

public interface IReportExportService
{
    public string ExportAsCsv<T>(List<T> data, string path);
    
    public string ExportAsCsv(List<Component> data, string path);
    
    public string ExportAsJson<T>(List<T> data, string path);
    
    public string ExportAsPdf<T>(List<T> data, string path);
}