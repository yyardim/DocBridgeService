using DocBridgeService.Models;
using DocBridgeService.Services;
using Moq;
using Moq.Protected;
using System.Net;

public class ApiServiceTests
{
    [Fact]
    public async Task SendToRepoCentralAsync_ShouldPostFileDataAndReturnResponse()
    {
        // Arrange
        var fileData = new FileData
        {
            FileName = "test.xml",
            Content = "<test>content</test>",
            Location = "/mock-location"
        };

        var expectedResponse = new RepoCentralResponse
        {
            Success = true,
            Message = "File processed"
        };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{ \"Success\": true, \"Message\": \"File processed\" }}")
        };

        // Mocking HttpMessageHandler to intercept the HttpClient call
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var apiService = new ApiService(httpClient);

        // Act
        var result = await apiService.SendToRepoCentralAsync(fileData);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("File processed", result.Message);

        // Verifying that the HttpClient call was made
        httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post && req.RequestUri == new System.Uri("https://repocentral/api/files")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
