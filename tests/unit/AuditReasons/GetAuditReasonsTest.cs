using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.AuditReasons
{
    public class GetAuditReasonsTest
    {
        [Fact]
        public async Task GetAuditReasonsAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var auditReasons = new Client.AuditReasons()
            {
                DeleteEntry = new List<WAuditReason>()
                {
                    new WAuditReason()
                    {
                        Id = 1,
                        Name = "first delete entry audit reason"
                    },
                    new WAuditReason()
                    {
                        Id = 2,
                        Name = "second delete entry audit reason"
                    }
                },
                ExportDocument = new List<WAuditReason>()
                {
                    new WAuditReason()
                    {
                        Id = 10,
                        Name = "export document audit reason"
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
                   Content = new StringContent(JsonConvert.SerializeObject(auditReasons))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.AuditReasonsClient.GetAuditReasonsAsync(repoId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(auditReasons.DeleteEntry.Count, result.DeleteEntry.Count);
            var deleteEntryAuditReasonsEnumerator = auditReasons.DeleteEntry.GetEnumerator();
            var resultEnumerator = result.DeleteEntry.GetEnumerator();
            while (deleteEntryAuditReasonsEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(deleteEntryAuditReasonsEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(deleteEntryAuditReasonsEnumerator.Current.Name, resultEnumerator.Current.Name);
            }

            Assert.Equal(auditReasons.ExportDocument.Count, result.ExportDocument.Count);
            var exportDocumentAuditReasonsEnumerator = auditReasons.ExportDocument.GetEnumerator();
            resultEnumerator = result.ExportDocument.GetEnumerator();
            while (exportDocumentAuditReasonsEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(exportDocumentAuditReasonsEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(exportDocumentAuditReasonsEnumerator.Current.Name, resultEnumerator.Current.Name);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/AuditReasons");

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
        public async Task GetAuditReasonsAsync_ReturnErrorStatusCode()
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.AuditReasonsClient.GetAuditReasonsAsync(repoId));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/AuditReasons");

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
