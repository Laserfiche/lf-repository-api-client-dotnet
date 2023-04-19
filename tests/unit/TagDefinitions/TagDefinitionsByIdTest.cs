using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.TagDefinitions
{
    public class TagDefinitionsByIdTest
    {
        [Fact]
        public async Task GetTagDefinitionByIdAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

            WTagInfo tagInfo = new WTagInfo()
            {
                Id = 1,
                Name = "TagName",
                Description = "Description",
                IsSecure = true,
                Watermark = new Watermark()
                {
                    WatermarkText = "Watermark Text",
                    WatermarkTextSize = 3,
                    IsWatermarkMandatory = true,
                    WatermarkIntensity = 4,
                    WatermarkPosition = WatermarkPosition.TopLeft,
                    WatermarkRotationAngle = 5
                }
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(tagInfo)),
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TagDefinitionsClient.GetTagDefinitionByIdAsync(repoId, tagInfo.Id).ConfigureAwait(false);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(tagInfo.Id, result.Id);
            Assert.Equal(tagInfo.Name, result.Name);
            Assert.Equal(tagInfo.Description, result.Description);
            Assert.Equal(tagInfo.IsSecure, result.IsSecure);
            Assert.Equal(tagInfo.Watermark.WatermarkText, result.Watermark.WatermarkText);
            Assert.Equal(tagInfo.Watermark.WatermarkTextSize, result.Watermark.WatermarkTextSize);
            Assert.Equal(tagInfo.Watermark.IsWatermarkMandatory, result.Watermark.IsWatermarkMandatory);
            Assert.Equal(tagInfo.Watermark.WatermarkIntensity, result.Watermark.WatermarkIntensity);
            Assert.Equal(tagInfo.Watermark.WatermarkPosition, result.Watermark.WatermarkPosition);
            Assert.Equal(tagInfo.Watermark.WatermarkRotationAngle, result.Watermark.WatermarkRotationAngle);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TagDefinitions/{tagInfo.Id}");

            handlerMock.Protected().Verify(
               "SendAsync",
            Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetTagDefinitionByIdAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var tagDefinition = 123123;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.TagDefinitionsClient.GetTagDefinitionByIdAsync(repoId, tagDefinition).ConfigureAwait(false)).ConfigureAwait(false);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TagDefinitions/{tagDefinition}");

            handlerMock.Protected().Verify(
               "SendAsync",
            Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
