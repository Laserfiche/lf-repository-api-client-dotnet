using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class GetEntryTagsTest
    {
        [Fact]
        public async Task GetTagsAssignedToEntryAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            ODataValueOfIListOfWTagInfo entryTags = new ODataValueOfIListOfWTagInfo()
            {
                Value = new List<WTagInfo>()
                {
                    new WTagInfo()
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
                    },
                    new WTagInfo()
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
                    }
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
                   Content = new StringContent(JsonConvert.SerializeObject(entryTags))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetTagsAssignedToEntryAsync(repoId, entryId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entryTags.Value.Count, result.Value.Count);
            var tagEnumerator = entryTags.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (tagEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(tagEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(tagEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(tagEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(tagEnumerator.Current.IsSecure, resultEnumerator.Current.IsSecure);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkText, resultEnumerator.Current.Watermark.WatermarkText);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkTextSize, resultEnumerator.Current.Watermark.WatermarkTextSize);
                Assert.Equal(tagEnumerator.Current.Watermark.IsWatermarkMandatory, resultEnumerator.Current.Watermark.IsWatermarkMandatory);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkIntensity, resultEnumerator.Current.Watermark.WatermarkIntensity);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkPosition, resultEnumerator.Current.Watermark.WatermarkPosition);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkRotationAngle, resultEnumerator.Current.Watermark.WatermarkRotationAngle);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/tags");

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
        public async Task GetTagsAssignedToEntryAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            var statusCode = HttpStatusCode.Unauthorized;

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
                   StatusCode = statusCode
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.GetTagsAssignedToEntryAsync(repoId, entryId));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/tags");

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
        public async Task GetTagsAssignedToEntryAsync_WithPreferHeader_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            string preferHeaderValue = "maxpagesize=1";
            string selectQueryParameter = "description";
            string orderbyQueryParameter = "name";
            int topQueryParameter = 1;
            int skipQueryParameter = 1;
            bool countQueryParameter = true;
            ODataValueOfIListOfWTagInfo entryTags = new ODataValueOfIListOfWTagInfo()
            {
                Value = new List<WTagInfo>()
                {
                    new WTagInfo()
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
                    }
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
                   Content = new StringContent(JsonConvert.SerializeObject(entryTags))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetTagsAssignedToEntryAsync(repoId, entryId, prefer: preferHeaderValue,
                select: selectQueryParameter, orderby: orderbyQueryParameter, top: topQueryParameter,
                skip: skipQueryParameter, count: countQueryParameter);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entryTags.Value.Count, result.Value.Count);
            var tagEnumerator = entryTags.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (tagEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(tagEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(tagEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(tagEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(tagEnumerator.Current.IsSecure, resultEnumerator.Current.IsSecure);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkText, resultEnumerator.Current.Watermark.WatermarkText);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkTextSize, resultEnumerator.Current.Watermark.WatermarkTextSize);
                Assert.Equal(tagEnumerator.Current.Watermark.IsWatermarkMandatory, resultEnumerator.Current.Watermark.IsWatermarkMandatory);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkIntensity, resultEnumerator.Current.Watermark.WatermarkIntensity);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkPosition, resultEnumerator.Current.Watermark.WatermarkPosition);
                Assert.Equal(tagEnumerator.Current.Watermark.WatermarkRotationAngle, resultEnumerator.Current.Watermark.WatermarkRotationAngle);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/tags?" +
                $"%24select={selectQueryParameter}&%24orderby={orderbyQueryParameter}&%24top={topQueryParameter}&" +
                $"%24skip={skipQueryParameter}&%24count={countQueryParameter.ToString().ToLower()}");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get  // we expected a GET request
                    && req.RequestUri == expectedUri // to this uri
                    && req.Headers.Contains("Prefer") // and Prefer header with maxpagesize value
                    && req.Headers.GetValues("Prefer").FirstOrDefault() == preferHeaderValue
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
