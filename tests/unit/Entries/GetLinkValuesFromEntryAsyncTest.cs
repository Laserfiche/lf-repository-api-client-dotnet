// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class GetLinkValuesFromEntryAsyncTest
    {
        [Fact]
        public async Task GetLinkValuesFromEntryAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

            WEntryLinkInfo entryLinkInfo = new WEntryLinkInfo()
            {
                LinkId = 1,
                SourceId = 2,
                SourceFullPath = "sourceFullPath",
                SourceLabel = "sourceLabel",
                TargetId = 3,
                TargetFullPath = "targetFullPath",
                TargetLabel = "targetLabel",
                Description = "description",
                LinkTypeId = 123,
                LinkTypeDescription = "linkTypeDescription",
                LinkProperties = new Dictionary<string, string>() { ["1"] = "2" },
                SourceLink = "sourceLink",
                TargetLink = "targetLink"
            };

            WEntryLinkInfo entryLinkInfo2 = new WEntryLinkInfo()
            {
                LinkId = 2,
                SourceId = 3,
                SourceFullPath = "sourceFullPath2",
                SourceLabel = "sourceLabel2",
                TargetId = 4,
                TargetFullPath = "targetFullPath2",
                TargetLabel = "targetLabel2",
                Description = "description2",
                LinkTypeId = 456,
                LinkTypeDescription = "linkTypeDescription2",
                LinkProperties = new Dictionary<string, string>() { ["3"] = "4" },
                SourceLink = "sourceLink2",
                TargetLink = "targetLink2"
            };

            IList<WEntryLinkInfo> links = new List<WEntryLinkInfo>() { entryLinkInfo, entryLinkInfo2 };

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
                    Content = new StringContent(JsonConvert.SerializeObject(new ODataValueOfIListOfWEntryLinkInfo() { Value = links }))
                })
                .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.EntriesClient.GetLinkValuesFromEntryAsync(repoId, entryId).ConfigureAwait(false);

            var result = response.Value;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(links.ElementAt(i).LinkId, result.ElementAt(i).LinkId);
                Assert.Equal(links.ElementAt(i).SourceId, result.ElementAt(i).SourceId);
                Assert.Equal(links.ElementAt(i).SourceFullPath, result.ElementAt(i).SourceFullPath);
                Assert.Equal(links.ElementAt(i).SourceLabel, result.ElementAt(i).SourceLabel);
                Assert.Equal(links.ElementAt(i).TargetId, result.ElementAt(i).TargetId);
                Assert.Equal(links.ElementAt(i).TargetFullPath, result.ElementAt(i).TargetFullPath);
                Assert.Equal(links.ElementAt(i).TargetLabel, result.ElementAt(i).TargetLabel);
                Assert.Equal(links.ElementAt(i).TargetLink, result.ElementAt(i).TargetLink);
                Assert.Equal(links.ElementAt(i).Description, result.ElementAt(i).Description);
                Assert.Equal(links.ElementAt(i).LinkTypeId, result.ElementAt(i).LinkTypeId);
                Assert.Equal(links.ElementAt(i).LinkTypeDescription, result.ElementAt(i).LinkTypeDescription);
                Assert.Equal(links.ElementAt(i).SourceLink, result.ElementAt(i).SourceLink);
                foreach (var x in links.ElementAt(i).LinkProperties)
                {
                    Assert.Equal(links.ElementAt(i).LinkProperties[x.Key], result.ElementAt(i).LinkProperties[x.Key]);
                }
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/links");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && req.RequestUri == expectedUri
                ), // to this uri
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetLinkValuesFromEntryAsync_AnyOther()
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
            await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.GetLinkValuesFromEntryAsync(repoId, entryId).ConfigureAwait(false)).ConfigureAwait(false);


            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/links");

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
        public async Task GetLinkValuesFromEntryAsync_ODataQueryOptions()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;

            WEntryLinkInfo entryLinkInfo = new WEntryLinkInfo()
            {
                LinkId = 1,
                SourceId = 2,
                SourceFullPath = "sourceFullPath",
                SourceLabel = "sourceLabel",
                TargetId = 3,
                TargetFullPath = "targetFullPath",
                TargetLabel = "targetLabel",
                Description = "description",
                LinkTypeId = 123,
                LinkTypeDescription = "linkTypeDescription",
                LinkProperties = new Dictionary<string, string>() { ["1"] = "2" },
                SourceLink = "sourceLink",
                TargetLink = "targetLink"
            };

            WEntryLinkInfo entryLinkInfo2 = new WEntryLinkInfo()
            {
                LinkId = 2,
                SourceId = 3,
                SourceFullPath = "sourceFullPath2",
                SourceLabel = "sourceLabel2",
                TargetId = 4,
                TargetFullPath = "targetFullPath2",
                TargetLabel = "targetLabel2",
                Description = "description2",
                LinkTypeId = 456,
                LinkTypeDescription = "linkTypeDescription2",
                LinkProperties = new Dictionary<string, string>() { ["3"] = "4" },
                SourceLink = "sourceLink2",
                TargetLink = "targetLink2"
            };

            IList<WEntryLinkInfo> links = new List<WEntryLinkInfo>() { entryLinkInfo, entryLinkInfo2 };

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
                    Content = new StringContent(JsonConvert.SerializeObject(new ODataValueOfIListOfWEntryLinkInfo()
                    {
                        Value = links
                    }))
                })
                .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetLinkValuesFromEntryAsync(repoId, entryId, prefer: "Prefer", select: "select", orderby: "orderBy", top: 1, skip: 1, count: true).ConfigureAwait(false);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/Entries/1/links?%24select=select&%24orderby=orderBy&%24top=1&%24skip=1&%24count=true");

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
