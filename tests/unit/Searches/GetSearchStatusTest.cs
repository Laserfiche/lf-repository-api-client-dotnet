using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Searches
{
    public class GetSearchStatusTest
    {
        [Fact]
        public async Task GetSearchStatusAsync_ReturnOkStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            var searchStatus = new OperationProgress()
            {
                OperationToken = searchToken,
                OperationType = "Search",
                PercentComplete = 100,
                Status = OperationStatus.Failed,
                Errors = new List<OperationErrorItem>()
                {
                    new OperationErrorItem() { ErrorMessage = "The search failed" }
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
                   Content = new StringContent(JsonConvert.SerializeObject(searchStatus)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.SearchesClient.GetSearchStatusAsync(repoId, searchToken);
            var result = response.Result;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(searchStatus.OperationToken, result.OperationToken);
            Assert.Equal(searchStatus.OperationType, result.OperationType);
            Assert.Equal(searchStatus.PercentComplete, result.PercentComplete);
            Assert.Equal(searchStatus.Status, result.Status);
            Assert.Equal(searchStatus.Errors.Count, result.Errors.Count);
            Assert.Equal(searchStatus.Errors.FirstOrDefault().ErrorMessage, result.Errors.FirstOrDefault()?.ErrorMessage);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}");

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
        public async Task GetSearchStatusAsync_ReturnCreatedStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            var searchStatus = new OperationProgress()
            {
                OperationToken = searchToken,
                OperationType = "Search",
                PercentComplete = 100,
                Status = OperationStatus.Completed,
                Errors = new List<OperationErrorItem>()
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
                   StatusCode = HttpStatusCode.Created,
                   Content = new StringContent(JsonConvert.SerializeObject(searchStatus)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.SearchesClient.GetSearchStatusAsync(repoId, searchToken);
            var result = response.Result;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(searchStatus.OperationToken, result.OperationToken);
            Assert.Equal(searchStatus.OperationType, result.OperationType);
            Assert.Equal(searchStatus.PercentComplete, result.PercentComplete);
            Assert.Equal(searchStatus.Status, result.Status);
            Assert.Equal(searchStatus.Errors.Count, result.Errors.Count);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}");

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
        public async Task GetSearchStatusAsync_ReturnAcceptedStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            var searchStatus = new OperationProgress()
            {
                OperationToken = searchToken,
                OperationType = "Search",
                PercentComplete = 25,
                Status = OperationStatus.InProgress,
                Errors = new List<OperationErrorItem>()
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
                   StatusCode = HttpStatusCode.Accepted,
                   Content = new StringContent(JsonConvert.SerializeObject(searchStatus)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.SearchesClient.GetSearchStatusAsync(repoId, searchToken);
            var result = response.Result;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(searchStatus.OperationToken, result.OperationToken);
            Assert.Equal(searchStatus.OperationType, result.OperationType);
            Assert.Equal(searchStatus.PercentComplete, result.PercentComplete);
            Assert.Equal(searchStatus.Status, result.Status);
            Assert.Equal(searchStatus.Errors.Count, result.Errors.Count);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}");

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
        public async Task GetSearchStatusAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.SearchesClient.GetSearchStatusAsync(repoId, searchToken));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}");

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
