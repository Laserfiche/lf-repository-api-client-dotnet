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

namespace Laserfiche.Repository.Api.Client.Test.LinkDefinitions
{
    public class LinkDefinitionsTest
    {
        [Fact]
        public async Task GetLinkDefinitionsAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

            EntryLinkTypeInfo linkType1 = new EntryLinkTypeInfo()
            {
                LinkTypeId = 1,
                SourceLabel = "linktype sourcelabel 1",
                TargetLabel = "linktype targetlabel 1",
                LinkTypeDescription = "linktype description 1"
            };
            EntryLinkTypeInfo linkType2 = new EntryLinkTypeInfo()
            {
                LinkTypeId = 2,
                SourceLabel = "linktype sourcelabel 2",
                TargetLabel = "linktype targetlabel 2",
                LinkTypeDescription = "linktype description 2"
            };
            ODataValueContextOfIListOfEntryLinkTypeInfo ret = new ODataValueContextOfIListOfEntryLinkTypeInfo()
            {
                Value = new List<EntryLinkTypeInfo>() { linkType1, linkType2 }
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
                    Content = new StringContent(JsonConvert.SerializeObject(ret))
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(repoId);
            var result = response.Value;
            Assert.Equal(2, result.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(ret.Value.ElementAt(i).LinkTypeId, result.ElementAt(i).LinkTypeId);
                Assert.Equal(ret.Value.ElementAt(i).SourceLabel, result.ElementAt(i).SourceLabel);
                Assert.Equal(ret.Value.ElementAt(i).TargetLabel, result.ElementAt(i).TargetLabel);
                Assert.Equal(ret.Value.ElementAt(i).LinkTypeDescription, result.ElementAt(i).LinkTypeDescription);
            }

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/LinkDefinitions");

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
        public async Task GetLinkDefinitionsAsync_ODataQueryOptions()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

            EntryLinkTypeInfo linkType1 = new EntryLinkTypeInfo()
            {
                LinkTypeId = 1,
                SourceLabel = "linktype sourcelabel 1",
                TargetLabel = "linktype targetlabel 1",
                LinkTypeDescription = "linktype description 1"
            };
            EntryLinkTypeInfo linkType2 = new EntryLinkTypeInfo()
            {
                LinkTypeId = 2,
                SourceLabel = "linktype sourcelabel 2",
                TargetLabel = "linktype targetlabel 2",
                LinkTypeDescription = "linktype description 2"
            };
            ODataValueContextOfIListOfEntryLinkTypeInfo ret = new ODataValueContextOfIListOfEntryLinkTypeInfo()
            {
                Value = new List<EntryLinkTypeInfo>() { linkType1, linkType2 }
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
                    Content = new StringContent(JsonConvert.SerializeObject(ret))
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            _ = await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(repoId, prefer: "Prefer", select: "select", orderby: "orderby", top: 1, skip: 2, count: true);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/LinkDefinitions?%24select=select&%24orderby=orderby&%24top=1&%24skip=2&%24count=true");

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
        public async Task GetLinkDefinitionsAsync_AnyOther()
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
            await Assert.ThrowsAsync<ApiException>(async () => await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(repoId));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/LinkDefinitions");

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
