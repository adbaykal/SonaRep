using System.Data;
using System.Text.Json;
using SonaRep.Helper;
using SonaRep.Services.Models;

namespace SonaRep.Services;

public class ReportExportService: IReportExportService
{
    private readonly ICsvHelper _csvHelper;
    
    public ReportExportService(ICsvHelper csvHelper)
    {
        _csvHelper = csvHelper;
    }
    public string ExportAsCsv<T>(List<T> data, string path)
    {
        _csvHelper.SaveToCsv(data,path);
        return path;
    }

    public string ExportAsCsv(List<Component> data, string path)
    {
        var table = new DataTable();
        
        table.Columns.Add("id");
        table.Columns.Add("key");
        table.Columns.Add("name");
        table.Columns.Add("qualifier");
        
        foreach (var measure in data.SelectMany(component => component.measures.Where(measure => !table.Columns.Contains(measure.metric))))
        {
            table.Columns.Add(measure.metric);
        }

        foreach (var component in data)
        {
            var row = table.NewRow();

            row["id"] = component.id;
            row["key"] = component.key;
            row["name"] = component.name;
            row["qualifier"] = component.qualifier;

            foreach (var measure in component.measures)
            {
                row[measure.metric] = measure.value;
            }

            table.Rows.Add(row);
        }
        _csvHelper.SaveToCsv(table,path);
        return path;
    }

    public string ExportAsJson<T>(List<T> data, string path)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path,json);
        return path;
    }

    public string ExportAsPdf<T>(List<T> data, string path)
    {
        throw new NotImplementedException();
    }
}