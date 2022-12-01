using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class ExportDocumentAsyncTest
    {
        public class MemoryStreamJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(MemoryStream).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var bytes = serializer.Deserialize<byte[]>(reader);
                return bytes != null ? new MemoryStream(bytes) : new MemoryStream();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var bytes = ((MemoryStream)value).ToArray();
                serializer.Serialize(writer, bytes);
            }
        }

        [Fact]
        public async Task ExportDocumentAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    FileResponse fileResponse = new FileResponse(200, null, stream, null, null);

                    HttpResponseMessage httpResponse = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject("hello world", Formatting.Indented, new MemoryStreamJsonConverter()))
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
                        .ReturnsAsync(httpResponse)
                        .Verifiable();

                    // use real http client with mocked handler here
                    var httpClient = new HttpClient(handlerMock.Object)
                    {
                        BaseAddress = new Uri(baseAddress),
                    };

                    var client = new RepositoryApiClient(httpClient);

                    // ACT
                    var swaggerResponse = await client.EntriesClient.ExportDocumentAsync(repoId, entryId, range : "bytes=0-200, 300-400");

                    // ASSERT
                    string result = "";
                    using (var reader = new StreamReader(swaggerResponse.Stream))
                    {
                        result = reader.ReadToEnd();
                    }

                    Assert.NotNull(swaggerResponse);
                    Assert.Equal(200, swaggerResponse.StatusCode);
                    Assert.Equal("\"hello world\"", result);

                    // also check the 'http' call was like we expected it
                    var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Document/edoc");

                    handlerMock.Protected().Verify(
                       "SendAsync",
                       Times.Exactly(1), // we expected a single external request
                       ItExpr.Is<HttpRequestMessage>(req =>
                          req.Method == HttpMethod.Get  // we expected a GET request
                          && req.RequestUri == expectedUri // to this uri
                          && req.Headers.Range.Ranges.ElementAt(0).From == 0 // with range header
                          && req.Headers.Range.Ranges.ElementAt(0).To == 200 // with range header
                          && req.Headers.Range.Ranges.ElementAt(1).From == 300 // with range header
                          && req.Headers.Range.Ranges.ElementAt(1).To == 400 // with range header
                        ), 
                       ItExpr.IsAny<CancellationToken>()
                    );
                }
            }
        }

        [Fact]
        public async Task ExportDocumentAsync_206()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    FileResponse fileResponse = new FileResponse(200, null, stream, null, null);

                    HttpResponseMessage httpResponse = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.PartialContent,
                        Content = new StringContent(JsonConvert.SerializeObject("hello world", Formatting.Indented, new MemoryStreamJsonConverter()))
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
                        .ReturnsAsync(httpResponse)
                        .Verifiable();

                    // use real http client with mocked handler here
                    var httpClient = new HttpClient(handlerMock.Object)
                    {
                        BaseAddress = new Uri(baseAddress),
                    };

                    var client = new RepositoryApiClient(httpClient);

                    // ACT
                    var swaggerResponse = await client.EntriesClient.ExportDocumentAsync(repoId, entryId, range : "bytes=0-200, 300-400");

                    // ASSERT
                    string result = "";
                    using (var reader = new StreamReader(swaggerResponse.Stream))
                    {
                        result = reader.ReadToEnd();
                    }

                    Assert.NotNull(swaggerResponse);
                    Assert.Equal(206, swaggerResponse.StatusCode);
                    Assert.Equal("\"hello world\"", result);

                    // also check the 'http' call was like we expected it
                    var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Document/edoc");

                    handlerMock.Protected().Verify(
                       "SendAsync",
                       Times.Exactly(1), // we expected a single external request
                       ItExpr.Is<HttpRequestMessage>(req =>
                          req.Method == HttpMethod.Get  // we expected a GET request
                          && req.RequestUri == expectedUri // to this uri
                          && req.Headers.Range.Ranges.ElementAt(0).From == 0 // with range header
                          && req.Headers.Range.Ranges.ElementAt(0).To == 200 // with range header
                          && req.Headers.Range.Ranges.ElementAt(1).From == 300 // with range header
                          && req.Headers.Range.Ranges.ElementAt(1).To == 400 // with range header
                        ), 
                       ItExpr.IsAny<CancellationToken>()
                    );
                }
            }
        }

        [Fact]
        public async Task ExportDocumentAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

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

            var client = new RepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.ExportDocumentAsync(repoId, entryId));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Document/edoc");

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
