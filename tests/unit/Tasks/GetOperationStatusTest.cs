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

namespace Laserfiche.Repository.Api.Client.Test.Tasks
{
    public class GetOperationStatusTest
    {
        [Fact]
        public async Task GetOperationStatusAndProgressAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            OperationProgress operationProgress = new OperationProgress()
            {
                OperationToken = "operationToken",
                OperationType = "type",
                PercentComplete = 100,
                Status = OperationStatus.Completed
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
                   Content = new StringContent(JsonConvert.SerializeObject(operationProgress))
               })
               .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.TasksClient.GetOperationStatusAndProgressAsync(repoId, operationProgress.OperationToken).ConfigureAwait(false);

            // ASSERT
            Assert.Equal(operationProgress.OperationToken, swaggerResponse.OperationToken);
            Assert.Equal(operationProgress.OperationType, swaggerResponse.OperationType);
            Assert.Equal(operationProgress.Status, swaggerResponse.Status);
            Assert.Equal(operationProgress.PercentComplete, swaggerResponse.PercentComplete);

            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Tasks/{operationProgress.OperationToken}");

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
        public async Task GetOperationStatusAndProgressAsync_202()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            OperationErrorItem error = new OperationErrorItem() { ObjectId = 1, ErrorMessage = "ErrorMessage" };
            OperationProgress operationProgress = new OperationProgress()
            {
                OperationToken = "operationToken",
                OperationType = "type",
                PercentComplete = 100,
                Status = OperationStatus.Failed,
                Errors = new List<OperationErrorItem>() { error }
            };

            var mockHttpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Accepted,
                Content = new StringContent(JsonConvert.SerializeObject(operationProgress)),
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
            var progress = await client.TasksClient.GetOperationStatusAndProgressAsync(repoId, operationProgress.OperationToken).ConfigureAwait(false);

            // ASSERT
            Assert.Equal(operationProgress.OperationToken, progress.OperationToken);
            Assert.Equal(operationProgress.OperationType, progress.OperationType);
            Assert.Equal(operationProgress.Status, progress.Status);
            Assert.Equal(operationProgress.PercentComplete, progress.PercentComplete);
            Assert.Equal(error.ObjectId, progress.Errors.First().ObjectId);
            Assert.Equal(error.ErrorMessage, progress.Errors.First().ErrorMessage);

            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Tasks/{operationProgress.OperationToken}");

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
        public async Task GetOperationStatusAndProgressAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var mockHttpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            var operationToken = "operationToken";

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
            await Assert.ThrowsAsync<ApiException>(async () => await client.TasksClient.GetOperationStatusAndProgressAsync(repoId, operationToken).ConfigureAwait(false)).ConfigureAwait(false);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Tasks/{operationToken}");

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
