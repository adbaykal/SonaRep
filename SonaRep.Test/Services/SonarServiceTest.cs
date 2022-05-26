using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SonaRep.Services;
using Xunit;

namespace SonaRep.Test.Services;

public class SonarServiceTest
{
    private readonly Mock<ILogger<SonarService>> _logger;
    private readonly Mock<IHttpClientFactory> _httpClientFactory;
    
    public SonarServiceTest()
    {
        _logger = new Mock<ILogger<SonarService>>();
        _httpClientFactory = new Mock<IHttpClientFactory>();
    }
    
    [Fact]
    public void GetFavoritesAsync_ShouldReturnNull_WhenServiceResultIsNotSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.BadGateway, "");
       
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = sonarService.GetFavoritesAsync(It.IsAny<string>()).Result;
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFavoritesAsync_ShouldReturnList_WhenServiceResultIsSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.OK,"{\"paging\":{\"pageIndex\":1,\"pageSize\":100,\"total\":10},\"favorites\":[{\"organization\":\"getirdev\",\"key\":\"getirdev_payment-payout-callback-consumer\",\"name\":\"payment-payout-callback-consumer\",\"qualifier\":\"TRK\"}]}");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = sonarService.GetFavoritesAsync(It.IsAny<string>()).Result;
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
    
    [Fact]
    public void GetProjectDetailsAsyncWithoutMetrics_ShouldReturnList_WhenServiceResultIsSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.OK, 
            "{\r\n    \"component\": {\r\n        \"id\": \"AXu67Ot5wcV2gYdlgaXF\",\r\n        \"key\": \"getirdev_sap-integration-publisher\",\r\n        \"name\": \"sap-integration-publisher\",\r\n        \"qualifier\": \"TRK\",\r\n        \"measures\": [\r\n            {\r\n                \"metric\": \"coverage\",\r\n                \"value\": \"0.0\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"reliability_rating\",\r\n                \"value\": \"1.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"code_smells\",\r\n                \"value\": \"11\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"duplicated_lines_density\",\r\n                \"value\": \"7.9\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"security_rating\",\r\n                \"value\": \"1.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"new_coverage\",\r\n                \"periods\": [\r\n                    {\r\n                        \"index\": 1,\r\n                        \"value\": \"0.0\",\r\n                        \"bestValue\": false\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"metric\": \"sqale_debt_ratio\",\r\n                \"value\": \"0.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"effort_to_reach_maintainability_rating_a\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"security_hotspots\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"critical_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"blocker_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"alert_status\",\r\n                \"value\": \"ERROR\"\r\n            },\r\n            {\r\n                \"metric\": \"major_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"vulnerabilities\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"new_maintainability_rating\",\r\n                \"periods\": [\r\n                    {\r\n                        \"index\": 1,\r\n                        \"value\": \"1.0\",\r\n                        \"bestValue\": true\r\n                    }\r\n                ]\r\n            }\r\n        ]\r\n    }\r\n}");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = 
            sonarService.GetProjectDetailsAsync(It.IsAny<string>(),It.IsAny<string>()).Result;
        // Assert
        Assert.NotNull(result);
        Assert.Equal("sap-integration-publisher", result?.name);
        Assert.NotEmpty(result.measures);
    }
    
    [Fact]
    public void GetProjectDetailsAsyncWithoutMetrics_ShouldReturnNull_WhenServiceResultIsNotSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.BadRequest, "");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = 
            sonarService.GetProjectDetailsAsync(It.IsAny<string>(),It.IsAny<string>()).Result;
        // Assert
        Assert.Null(result);
        
    }
    
    [Fact]
    public void GetProjectDetailsAsyncWithMetrics_ShouldReturnNull_WhenServiceResultIsNotSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.BadRequest, "");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = 
            sonarService.GetProjectDetailsAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>()).Result;
        // Assert
        Assert.Null(result);
        
    }
    
    [Fact]
    public void GetProjectDetailsAsyncWithMetrics_ShouldReturnString_WhenServiceResultIsSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.OK, 
            "{\r\n    \"component\": {\r\n        \"id\": \"AXu67Ot5wcV2gYdlgaXF\",\r\n        \"key\": \"getirdev_sap-integration-publisher\",\r\n        \"name\": \"sap-integration-publisher\",\r\n        \"qualifier\": \"TRK\",\r\n        \"measures\": [\r\n            {\r\n                \"metric\": \"coverage\",\r\n                \"value\": \"0.0\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"reliability_rating\",\r\n                \"value\": \"1.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"code_smells\",\r\n                \"value\": \"11\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"duplicated_lines_density\",\r\n                \"value\": \"7.9\",\r\n                \"bestValue\": false\r\n            },\r\n            {\r\n                \"metric\": \"security_rating\",\r\n                \"value\": \"1.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"new_coverage\",\r\n                \"periods\": [\r\n                    {\r\n                        \"index\": 1,\r\n                        \"value\": \"0.0\",\r\n                        \"bestValue\": false\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"metric\": \"sqale_debt_ratio\",\r\n                \"value\": \"0.0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"effort_to_reach_maintainability_rating_a\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"security_hotspots\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"critical_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"blocker_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"alert_status\",\r\n                \"value\": \"ERROR\"\r\n            },\r\n            {\r\n                \"metric\": \"major_violations\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"vulnerabilities\",\r\n                \"value\": \"0\",\r\n                \"bestValue\": true\r\n            },\r\n            {\r\n                \"metric\": \"new_maintainability_rating\",\r\n                \"periods\": [\r\n                    {\r\n                        \"index\": 1,\r\n                        \"value\": \"1.0\",\r\n                        \"bestValue\": true\r\n                    }\r\n                ]\r\n            }\r\n        ]\r\n    }\r\n}");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = 
            sonarService.GetProjectDetailsAsync(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>()).Result;
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    

    [Fact]
    public void ListMetricsAsync_ShouldReturnMetrics_WhenServiceResultIsSuccess()
    {
        // Arrange
        SetupHttpClient(HttpStatusCode.OK,
            "{\r\n    \"metrics\": [\r\n        {\r\n            \"id\": \"289\",\r\n            \"key\": \"new_technical_debt\",\r\n            \"type\": \"WORK_DUR\",\r\n            \"name\": \"Added Technical Debt\",\r\n            \"description\": \"Added technical debt\",\r\n            \"domain\": \"Maintainability\",\r\n            \"direction\": -1,\r\n            \"qualitative\": true,\r\n            \"hidden\": false\r\n        }\r\n    ],\r\n    \"total\": 112,\r\n    \"p\": 1,\r\n    \"ps\": 100\r\n}");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = sonarService.ListMetricsAsync(It.IsAny<string>()).Result;
        // Assert
        Assert.NotNull(result);
        Assert.Single(result.metrics);
    }
    
    [Fact]
    public void ListMetricsAsync_ShouldReturnNull_WhenServiceResultIsNotSuccess()
    {
        // Arrange
       SetupHttpClient(HttpStatusCode.BadRequest,"");
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = sonarService.ListMetricsAsync(It.IsAny<string>()).Result;
        // Assert
        Assert.Null(result);
    }

    private void SetupHttpClient(HttpStatusCode httpStatusCode, string content)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = new StringContent(content)
            });
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
    }
    
}