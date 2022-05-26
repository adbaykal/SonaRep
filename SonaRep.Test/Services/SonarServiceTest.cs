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
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            });
        
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
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
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    "{\"paging\":{\"pageIndex\":1,\"pageSize\":100,\"total\":10},\"favorites\":[{\"organization\":\"getirdev\",\"key\":\"getirdev_payment-payout-callback-consumer\",\"name\":\"payment-payout-callback-consumer\",\"qualifier\":\"TRK\"}]}"
                    )
            });
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        
        _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
        var sonarService = new SonarService(_logger.Object, _httpClientFactory.Object);
        // Act
        var result = sonarService.GetFavoritesAsync(It.IsAny<string>()).Result;
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}