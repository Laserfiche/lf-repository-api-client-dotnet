using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class DeleteEntryTest
    {
        [Fact]
        public async Task DeleteEntryInfoAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            var request = new DeleteEntryWithAuditReason()
            {
                AuditReasonId = 10,
                Comment = "a comment"
            };
            var acceptedOperation = new AcceptedOperation()
            {
                Token = "123-abc"
            };

            string requestContent = "";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // Capture the request body
               .Callback<HttpRequestMessage, CancellationToken>((httpRequestMessage, _) => requestContent = httpRequestMessage.Content.ReadAsStringAsync().Result)
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.Created,
                   Content = new StringContent(JsonConvert.SerializeObject(acceptedOperation))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.DeleteEntryInfoAsync(repoId, entryId, request);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(acceptedOperation.Token, result.Token);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Delete  // we expected a DELETE request
                   && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }

        [Fact]
        public async Task DeleteEntryInfoAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            var statusCode = HttpStatusCode.Unauthorized;
            var request = new DeleteEntryWithAuditReason()
            {
                AuditReasonId = 10,
                Comment = "a comment"
            };

            string requestContent = "";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // Capture the request body
               .Callback<HttpRequestMessage, CancellationToken>((httpRequestMessage, _) => requestContent = httpRequestMessage.Content.ReadAsStringAsync().Result)
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.DeleteEntryInfoAsync(repoId, entryId, request));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Delete  // we expected a DELETE request
                   && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }
    }
}
