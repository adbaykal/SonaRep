using System.Data;
using System.Text.Json;
using HandlebarsDotNet;
using SonaRep.Helper;
using SonaRep.Models;
using SonaRep.Services.Models;
using MetricModel = SonaRep.Models.MetricModel;

namespace SonaRep.Services;

public class ReportExportService: IReportExportService
{
    private readonly ICsvHelper _csvHelper;
    
    public ReportExportService(
        ICsvHelper csvHelper,
        ISonarService sonarService)
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

    public string ExportAsHtml(List<Component> data, string path, Models.MetricModel metricList)
    {
        
        var templatePath = Path.Combine(Environment.CurrentDirectory, "Template");
        var root = File.ReadAllText(Path.Combine(templatePath,"template.html"));
        var projectPartial = File.ReadAllText(Path.Combine(templatePath,"projectPartial.html"));
        var measurePartial = File.ReadAllText(Path.Combine(templatePath,"measurePartial.html"));
        
        Handlebars.RegisterTemplate("measure", measurePartial);
        Handlebars.RegisterTemplate("project", projectPartial);
        
        var template = Handlebars.Compile(root);
        var viewData = new
        {
            projects = new List<ProjectViewModel>()
        };

        foreach (var component in data)
        {
            
            var measureList = new List<MeasureViewModel>();
            foreach (var measure in component.measures)
            {
                var metric = metricList.metrics.FirstOrDefault(x => x.key.Equals(measure.metric));
                var measureViewModel = new MeasureViewModel(measure)
                {
                    name = metric == null ? "" : metric.name,
                    description = metric == null ? "" : metric.description,
                    type = metric == null ? "" : metric.type,
                };

                measureViewModel = FormatMeasure(measureViewModel);
                
                measureList.Add(measureViewModel);
            }

            var maintainabilityRating = measureList.FirstOrDefault(x => x.metric.Equals("sqale_rating"));

            var reliabilityRating = measureList.FirstOrDefault(x => x.metric.Equals("reliability_rating"));
            
            var securityRating = measureList.FirstOrDefault(x => x.metric.Equals("security_rating"));

            var projectViewModel = new ProjectViewModel()
            {
                id = component.id,
                key = component.key,
                measures = measureList,
                name = component.name,
                qualifier = component.qualifier,
                maintainabilityRating = maintainabilityRating,
                reliabilityRating = reliabilityRating,
                securityRating = securityRating
            };
            
            viewData.projects.Add(projectViewModel);
        }
        
        var result = template(viewData);
        
        File.WriteAllText(path,result);

        return path;
    }

    private Dictionary<string, MetricModel> ratingMatch = new()
    {
        {"1.0", new MetricModel(){value = "A",badgeClass= "success"}},
        {"2.0", new MetricModel(){value = "B",badgeClass="warning"}},
        {"3.0", new MetricModel(){value = "C",badgeClass="danger"}},
        {"4.0", new MetricModel(){value = "D",badgeClass="danger"}},
        {"5.0", new MetricModel(){value = "E",badgeClass="danger"}},
    };

    
    public MeasureViewModel FormatMeasure(MeasureViewModel model)
    {
        switch (model.type)
        {
            case "RATING":
                model.badgeClass = model.value!= null?ratingMatch[model.value].badgeClass:"secondary";
                model.formattedValue = model.value!= null?ratingMatch[model.value].value:"";
                break;
            case "PERCENT":
                model.badgeClass = "secondary";
                model.formattedValue = "%" + model.value ;
                break;
            case "LEVEL":
                model.badgeClass = model.value == "ERROR" ? "danger" : "success";
                model.formattedValue = model.value ;
                break;
            default:
                model.badgeClass = "secondary";
                model.formattedValue = model.value;
                break;
        }

        return model;
    }
}