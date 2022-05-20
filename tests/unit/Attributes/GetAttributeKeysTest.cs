using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Attributes
{
    public class GetAttributeKeysTest
    {
        [Fact]
        public async Task GetTrusteeAttributeKeyValuePairsAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var attributeKeys = new ODataValueOfListOfAttribute()
            {
                Value = new List<Attribute>() {
                    new Attribute() { Key = "attributekey1", Value = "value1"}, 
                    new Attribute() { Key = "attributekey2", Value = "value2"},
                    new Attribute() { Key = "attributekey3", Value = "value3"}, 
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
                   Content = new StringContent(JsonConvert.SerializeObject(attributeKeys))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(repoId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(attributeKeys.Value.Count, result.Value.Count);
            var attributeKeysEnumerator = attributeKeys.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (attributeKeysEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(attributeKeysEnumerator.Current.Key, resultEnumerator.Current.Key);
                Assert.Equal(attributeKeysEnumerator.Current.Value, resultEnumerator.Current.Value);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Attributes");

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
        public async Task GetTrusteeAttributeKeyValuePairsAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(repoId));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Attributes");

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
        public async Task GetTrusteeAttributeKeyValuePairsAsync_WithQueryParameter_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var attributeKeys = new ODataValueOfListOfAttribute()
            {
                Value = new List<Attribute>() {
                    new Attribute() { Key = "attributekey1", Value = "value1"},
                    new Attribute() { Key = "attributekey2", Value = "value2"},
                    new Attribute() { Key = "attributekey3", Value = "value3"},
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
                   Content = new StringContent(JsonConvert.SerializeObject(attributeKeys))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(repoId, everyone: true);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(attributeKeys.Value.Count, result.Value.Count);
            var attributeKeysEnumerator = attributeKeys.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (attributeKeysEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(attributeKeysEnumerator.Current.Key, resultEnumerator.Current.Key);
                Assert.Equal(attributeKeysEnumerator.Current.Value, resultEnumerator.Current.Value);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Attributes?everyone=true");

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
