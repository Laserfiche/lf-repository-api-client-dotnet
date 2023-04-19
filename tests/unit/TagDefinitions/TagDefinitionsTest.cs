using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.TagDefinitions
{
    public class TagDefinitionsTest
    {
        [Fact]
        public async Task GetTagDefinitionsAsync_200()
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
            WTagInfo tagInfo2 = new WTagInfo()
            {
                Id = 2,
                Name = "TagName2",
                Description = "Description2",
                IsSecure = false,
                Watermark = new Watermark()
                {
                    WatermarkText = "Watermark Text2",
                    WatermarkTextSize = 6,
                    IsWatermarkMandatory = false,
                    WatermarkIntensity = 7,
                    WatermarkPosition = WatermarkPosition.TopRight,
                    WatermarkRotationAngle = 8
                }
            };
            IList<WTagInfo> tags = new List<WTagInfo>() { tagInfo, tagInfo2 };

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
                    Content = new StringContent(JsonConvert.SerializeObject(new ODataValueOfIListOfWTagInfo() { Value = tags}))
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(repoId).ConfigureAwait(false);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal(tagInfo.Id, result.Value.ElementAt(0).Id);
            Assert.Equal(tagInfo.Description, result.Value.ElementAt(0).Description);
            Assert.Equal(tagInfo.Name, result.Value.ElementAt(0).Name);
            Assert.Equal(tagInfo.IsSecure, result.Value.ElementAt(0).IsSecure);
            Assert.Equal(tagInfo.Watermark.WatermarkTextSize, result.Value.ElementAt(0).Watermark.WatermarkTextSize);
            Assert.Equal(tagInfo.Watermark.IsWatermarkMandatory, result.Value.ElementAt(0).Watermark.IsWatermarkMandatory);
            Assert.Equal(tagInfo.Watermark.WatermarkIntensity, result.Value.ElementAt(0).Watermark.WatermarkIntensity);
            Assert.Equal(tagInfo.Watermark.WatermarkPosition, result.Value.ElementAt(0).Watermark.WatermarkPosition);
            Assert.Equal(tagInfo.Watermark.WatermarkRotationAngle, result.Value.ElementAt(0).Watermark.WatermarkRotationAngle);
            Assert.Equal(tagInfo.Watermark.WatermarkText, result.Value.ElementAt(0).Watermark.WatermarkText);
            Assert.Equal(tagInfo2.Id, result.Value.ElementAt(1).Id);
            Assert.Equal(tagInfo2.Description, result.Value.ElementAt(1).Description);
            Assert.Equal(tagInfo2.Name, result.Value.ElementAt(1).Name);
            Assert.Equal(tagInfo2.IsSecure, result.Value.ElementAt(1).IsSecure);
            Assert.Equal(tagInfo2.Watermark.WatermarkTextSize, result.Value.ElementAt(1).Watermark.WatermarkTextSize);
            Assert.Equal(tagInfo2.Watermark.IsWatermarkMandatory, result.Value.ElementAt(1).Watermark.IsWatermarkMandatory);
            Assert.Equal(tagInfo2.Watermark.WatermarkIntensity, result.Value.ElementAt(1).Watermark.WatermarkIntensity);
            Assert.Equal(tagInfo2.Watermark.WatermarkPosition, result.Value.ElementAt(1).Watermark.WatermarkPosition);
            Assert.Equal(tagInfo2.Watermark.WatermarkRotationAngle, result.Value.ElementAt(1).Watermark.WatermarkRotationAngle);
            Assert.Equal(tagInfo2.Watermark.WatermarkText, result.Value.ElementAt(1).Watermark.WatermarkText);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TagDefinitions");

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
        public async Task GetTagDefinitionsAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

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
            await Assert.ThrowsAsync<ApiException>(async () => await client.TagDefinitionsClient.GetTagDefinitionsAsync(repoId).ConfigureAwait(false)).ConfigureAwait(false);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TagDefinitions");

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
        public async Task GetTagDefinitionsAsync_ODataQueryOptions()
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
            WTagInfo tagInfo2 = new WTagInfo()
            {
                Id = 2,
                Name = "TagName2",
                Description = "Description2",
                IsSecure = false,
                Watermark = new Watermark()
                {
                    WatermarkText = "Watermark Text2",
                    WatermarkTextSize = 6,
                    IsWatermarkMandatory = false,
                    WatermarkIntensity = 7,
                    WatermarkPosition = WatermarkPosition.TopRight,
                    WatermarkRotationAngle = 8
                }
            };
            IList<WTagInfo> tags = new List<WTagInfo>() { tagInfo, tagInfo2 };

            var mockHttpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new ODataValueOfIListOfWTagInfo() { Value = tags }))
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
                .ReturnsAsync(mockHttpResponseMessage)
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
                
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            _ = await client.TagDefinitionsClient.GetTagDefinitionsAsync(repoId, prefer:"Prefer", select:"select", orderby:"orderBy",top:1, skip:1, count:true).ConfigureAwait(false);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/TagDefinitions?%24select=select&%24orderby=orderBy&%24top=1&%24skip=1&%24count=true");

            handlerMock.Protected().Verify(
               "SendAsync",
            Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
                  && req.Headers.GetValues("Prefer").Contains("Prefer") // with the prefer header set to "Prefer"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
