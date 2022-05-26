using System.Collections.Generic;
using Moq;
using SonaRep.Helper;
using SonaRep.Models;
using SonaRep.Services;
using SonaRep.Services.Models;
using Xunit;
using MetricModel = SonaRep.Services.Models.MetricModel;

namespace SonaRep.Test.Services;

public class ReportExportServiceTest
{
    private readonly Mock<ICsvHelper> _csvHelper;
    
    public ReportExportServiceTest()
    {
        _csvHelper = new Mock<ICsvHelper>();
    }
    
    [Fact]
    public void ExportAsCsv_Should_Return_Csv_String()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var data = new List<Component>();
        var expectedPath = "path";
        
        // Act
        var actualCsv = reportExportService.ExportAsCsv(data,expectedPath);
        
        // Assert
        Assert.Equal(expectedPath, actualCsv);
    }
    
    [Fact]
    public void ExportAsJson_Should_Return_Json_Path()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var data = new List<Component>();
        var expectedPath = "path";
        
        // Act
        var actualJson = reportExportService.ExportAsJson(data,expectedPath);
        
        // Assert
        Assert.Equal(expectedPath, actualJson);
    }
    
    [Fact]
    public void ExportAsHtml_Should_Return_Html_Path()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var data = new List<Component>();
        var expectedPath = "path";
        var metricModel = new MetricModel();

        // Act
        var actualHtml = reportExportService.ExportAsHtml(data,expectedPath,metricModel);
        
        // Assert
        Assert.Equal(expectedPath, actualHtml);
    }
    
    
    [Fact]
    public void FormatMeasure_ShouldFill_Measure_With_FormattedValue_When_ModelType_Is_Rating()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var measureViewModel = new MeasureViewModel(new Measure()
        {
            metric = "metric1",
            value = "4.0",
            bestValue = false
        });
        measureViewModel.type = "RATING";
        var expectedBadgeClass = "danger";
        var expectedFormattedValue = "D";
        // Act
        var actualModel = reportExportService.FormatMeasure(measureViewModel);
        
        // Assert
        Assert.Equal(expectedBadgeClass, actualModel.badgeClass);
        Assert.Equal(expectedFormattedValue, actualModel.formattedValue);
    }
   
    [Fact]
    public void FormatMeasure_ShouldFill_Measure_With_FormattedValue_When_ModelType_Is_Percent()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var measureViewModel = new MeasureViewModel(new Measure()
        {
            metric = "metric1",
            value = "4.0",
            bestValue = false
        });
        measureViewModel.type = "PERCENT";
        var expectedBadgeClass = "secondary";
        var expectedFormattedValue = "%4.0";
        // Act
        var actualModel = reportExportService.FormatMeasure(measureViewModel);
        
        // Assert
        Assert.Equal(expectedBadgeClass, actualModel.badgeClass);
        Assert.Equal(expectedFormattedValue, actualModel.formattedValue);
    }
    
    [Fact]
    public void FormatMeasure_ShouldFill_Measure_With_FormattedValue_When_ModelType_Is_Level()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var measureViewModel = new MeasureViewModel(new Measure()
        {
            metric = "metric1",
            value = "ERROR",
            bestValue = false
        });
        measureViewModel.type = "LEVEL";
        var expectedBadgeClass = "danger";
        var expectedFormattedValue = "ERROR";
        // Act
        var actualModel = reportExportService.FormatMeasure(measureViewModel);
        
        // Assert
        Assert.Equal(expectedBadgeClass, actualModel.badgeClass);
        Assert.Equal(expectedFormattedValue, actualModel.formattedValue);
    }
    
    [Fact]
    public void FormatMeasure_ShouldFill_Measure_With_FormattedValue_When_ModelType_Is_Default()
    {
        // Arrange
        var reportExportService = new ReportExportService(_csvHelper.Object);
        var measureViewModel = new MeasureViewModel(new Measure()
        {
            metric = "metric1",
            value = "value",
            bestValue = false
        });
        measureViewModel.type = "SOMEDIFFERENTTYPE";
        var expectedBadgeClass = "secondary";
        var expectedFormattedValue = "value";
        // Act
        var actualModel = reportExportService.FormatMeasure(measureViewModel);
        
        // Assert
        Assert.Equal(expectedBadgeClass, actualModel.badgeClass);
        Assert.Equal(expectedFormattedValue, actualModel.formattedValue);
    }
    
}