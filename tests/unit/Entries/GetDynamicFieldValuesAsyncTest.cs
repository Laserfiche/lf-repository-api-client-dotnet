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
    public class GetDynamicFieldValuesAsyncTest
    {
        [Fact]
        public async Task GetDynamicFieldValuesAsync_200()
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
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new Dictionary<string, List<string>>()
                    {
                        ["additionalProp1"] = new List<string>() { "additionalValue1" }
                    }
                    ))
                })
                .Verifiable();

            Dictionary<string, string> fieldValsDict = new Dictionary<string, string>(){ ["1"] = "2" };

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.GetDynamicFieldValuesAsync(repoId, entryId, new GetDynamicFieldLogicValueRequest()
                {
                    TemplateId = 1,
                    FieldValues = fieldValsDict
            }
            );

            // ASSERT
            Assert.NotNull(swaggerResponse);
            Assert.Equal(200, swaggerResponse.StatusCode);
            Assert.Equal(1, swaggerResponse.Result.Count);
            Assert.True(swaggerResponse.Result.ElementAt(0).Value.Contains("additionalValue1"));
            Assert.Equal(1, swaggerResponse.Result.ElementAt(0).Value.Count);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/fields/GetDynamicFieldLogicValue");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post// we expected a GET request
                    && req.RequestUri == expectedUri // to this uri
                    && req.Content.ToString() == new StringContent(JsonConvert.SerializeObject(new GetDynamicFieldLogicValueRequest()
                    {
                        TemplateId = 1,
                        FieldValues = fieldValsDict
                    })).ToString()
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }       
        

        [Fact]
        public async Task GetDynamicFieldValuesAsync_AnyOther()
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

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.GetDynamicFieldValuesAsync(repoId, entryId, new GetDynamicFieldLogicValueRequest()
            {
                TemplateId = 1,
                FieldValues = new Dictionary<string, string>() { ["1"] = "2" }
            }
            ));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/fields/GetDynamicFieldLogicValue");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
