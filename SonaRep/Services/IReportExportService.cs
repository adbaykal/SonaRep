using SonaRep.Services.Models;

namespace SonaRep.Services;

public interface IReportExportService
{
    
    public string ExportAsCsv(List<Component> data, string path);
    
    public string ExportAsJson<T>(List<T> data, string path);
    
    public string ExportAsHtml(List<Component> data, string path, MetricModel metricList);
    
    public Task<string> ExportAsPng(List<Component> data, string path, MetricModel metricList);
    public Task<string> ExportAsPdf(List<Component> data, string path, MetricModel metricList);
}