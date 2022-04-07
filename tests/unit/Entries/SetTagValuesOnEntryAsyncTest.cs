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

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class SetTagValuesOnEntryAsyncTest
    {
        [Fact]
        public async Task AssignTagsAsync_Return200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

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
                    Content = new StringContent(JsonConvert.SerializeObject(new ODataValueOfIListOfWTagInfo() { Value = tags }))
                })
                .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.AssignTagsAsync(repoId, entryId, new PutTagRequest()
            {
                Tags = new List<string>() { tagInfo.Name, tagInfo2.Name }
            });
            var result = swaggerResponse.Result.Value;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(tags.ElementAt(i).Name, result.ElementAt(i).Name);
                Assert.Equal(tags.ElementAt(i).Description, result.ElementAt(i).Description);
                Assert.Equal(tags.ElementAt(i).IsSecure, result.ElementAt(i).IsSecure);
                Assert.Equal(tags.ElementAt(i).Id, result.ElementAt(i).Id);
                Assert.Equal(tags.ElementAt(i).Watermark.WatermarkTextSize, result.ElementAt(i).Watermark.WatermarkTextSize);
                Assert.Equal(tags.ElementAt(i).Watermark.IsWatermarkMandatory, result.ElementAt(i).Watermark.IsWatermarkMandatory);
                Assert.Equal(tags.ElementAt(i).Watermark.WatermarkIntensity, result.ElementAt(i).Watermark.WatermarkIntensity);
                Assert.Equal(tags.ElementAt(i).Watermark.WatermarkPosition, result.ElementAt(i).Watermark.WatermarkPosition);
                Assert.Equal(tags.ElementAt(i).Watermark.WatermarkRotationAngle, result.ElementAt(i).Watermark.WatermarkRotationAngle);
                Assert.Equal(tags.ElementAt(i).Watermark.WatermarkText, result.ElementAt(i).Watermark.WatermarkText);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/tags");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
                  && req.Content.ToString() == new StringContent(JsonConvert.SerializeObject(new PutTagRequest()
                  {
                      Tags = new List<string>() { tagInfo.Name, tagInfo2.Name }
                  })).ToString()
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task AssignTagsAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

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
                    StatusCode = HttpStatusCode.InternalServerError
                })
                .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.AssignTagsAsync(repoId, entryId, new PutTagRequest()
            {
                Tags = new List<string>() { tagInfo.Name, tagInfo2.Name }
            }));


            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/tags");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
