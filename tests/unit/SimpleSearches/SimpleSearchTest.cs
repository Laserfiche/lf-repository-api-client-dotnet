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

namespace Laserfiche.Repository.Api.Client.Test.SimpleSearches
{
    public class SimpleSearchTest
    {
        [Fact]
        public async Task CreateSimpleSearchOperationAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            ODataValueOfIListOfODataGetEntryChildren entries = new ODataValueOfIListOfODataGetEntryChildren()
            {
                Value = new List<ODataGetEntryChildren>()
                {
                    new ODataGetEntryChildren()
                    {
                        Id = 100,
                        Name = "entry1",
                        ParentId = 1,
                        FullPath = @"\entry1",
                        FolderPath = @"\",
                        Creator = "",
                        CreationTime = DateTimeOffset.Parse("2021-01-01"),
                        LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                        EntryType = EntryType.Document,
                        TemplateName = null,
                        TemplateId = 0,
                        VolumeName = "default",
                        RowNumber = 1
                    },
                    new ODataGetEntryChildren()
                    {
                        Id = 101,
                        Name = "entry2",
                        ParentId = 1,
                        FullPath = @"\entry2",
                        FolderPath = @"\",
                        Creator = "",
                        CreationTime = DateTimeOffset.Parse("2021-01-01"),
                        LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                        EntryType = EntryType.Document,
                        TemplateName = null,
                        TemplateId = 0,
                        VolumeName = "default",
                        RowNumber = 2
                    }
                }
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
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonConvert.SerializeObject(entries))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var response = await client.CreateSimpleSearchOperationAsync(repoId, request: request);
            var result = response.Result;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entries.Value.Count, result.Value.Count);
            var entriesEnumerator = entries.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (entriesEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(entriesEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(entriesEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(entriesEnumerator.Current.ParentId, resultEnumerator.Current.ParentId);
                Assert.Equal(entriesEnumerator.Current.FullPath, resultEnumerator.Current.FullPath);
                Assert.Equal(entriesEnumerator.Current.FolderPath, resultEnumerator.Current.FolderPath);
                Assert.Equal(entriesEnumerator.Current.Creator, resultEnumerator.Current.Creator);
                Assert.Equal(entriesEnumerator.Current.CreationTime, resultEnumerator.Current.CreationTime);
                Assert.Equal(entriesEnumerator.Current.LastModifiedTime, resultEnumerator.Current.LastModifiedTime);
                Assert.Equal(entriesEnumerator.Current.EntryType, resultEnumerator.Current.EntryType);
                Assert.Equal(entriesEnumerator.Current.TemplateName, resultEnumerator.Current.TemplateName);
                Assert.Equal(entriesEnumerator.Current.TemplateId, resultEnumerator.Current.TemplateId);
                Assert.Equal(entriesEnumerator.Current.VolumeName, resultEnumerator.Current.VolumeName);
                Assert.Equal(entriesEnumerator.Current.RowNumber, resultEnumerator.Current.RowNumber);
                Assert.Equal(typeof(Document), resultEnumerator.Current.GetType());
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/SimpleSearches");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post  // we expected a POST request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }

        [Fact]
        public async Task CreateSimpleSearchOperationAsync_WithQueryParameters()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            ODataValueOfIListOfODataGetEntryChildren entries = new ODataValueOfIListOfODataGetEntryChildren()
            {
                Value = new List<ODataGetEntryChildren>()
                {
                    new ODataGetEntryChildren()
                    {
                        Id = 100,
                        Name = "entry1",
                        ParentId = 1,
                        FullPath = @"\entry1",
                        FolderPath = @"\",
                        Creator = "",
                        CreationTime = DateTimeOffset.Parse("2021-01-01"),
                        LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                        EntryType = EntryType.Document,
                        TemplateName = null,
                        TemplateId = 0,
                        VolumeName = "default",
                        RowNumber = 1
                    },
                    new ODataGetEntryChildren()
                    {
                        Id = 101,
                        Name = "entry2",
                        ParentId = 1,
                        FullPath = @"\entry2",
                        FolderPath = @"\",
                        Creator = "",
                        CreationTime = DateTimeOffset.Parse("2021-01-01"),
                        LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                        EntryType = EntryType.Document,
                        TemplateName = null,
                        TemplateId = 0,
                        VolumeName = "default",
                        RowNumber = 2
                    }
                }
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
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonConvert.SerializeObject(entries))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var response = await client.CreateSimpleSearchOperationAsync(repoId, select: "select", orderby: "orderby",
                count: true, fields: new List<string>() { "field1", "field2" }, formatFields: true, request: request, culture: "fr");
            var result = response.Result;

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entries.Value.Count, result.Value.Count);
            var entriesEnumerator = entries.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (entriesEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(entriesEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(entriesEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(entriesEnumerator.Current.ParentId, resultEnumerator.Current.ParentId);
                Assert.Equal(entriesEnumerator.Current.FullPath, resultEnumerator.Current.FullPath);
                Assert.Equal(entriesEnumerator.Current.FolderPath, resultEnumerator.Current.FolderPath);
                Assert.Equal(entriesEnumerator.Current.Creator, resultEnumerator.Current.Creator);
                Assert.Equal(entriesEnumerator.Current.CreationTime, resultEnumerator.Current.CreationTime);
                Assert.Equal(entriesEnumerator.Current.LastModifiedTime, resultEnumerator.Current.LastModifiedTime);
                Assert.Equal(entriesEnumerator.Current.EntryType, resultEnumerator.Current.EntryType);
                Assert.Equal(entriesEnumerator.Current.TemplateName, resultEnumerator.Current.TemplateName);
                Assert.Equal(entriesEnumerator.Current.TemplateId, resultEnumerator.Current.TemplateId);
                Assert.Equal(entriesEnumerator.Current.VolumeName, resultEnumerator.Current.VolumeName);
                Assert.Equal(entriesEnumerator.Current.RowNumber, resultEnumerator.Current.RowNumber);
                Assert.Equal(typeof(Document), resultEnumerator.Current.GetType());
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/SimpleSearches?%24select=select&%24orderby=orderby&%24count=true&fields=field1&fields=field2&formatFields=true&culture=fr");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post  // we expected a POST request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }

        [Fact]
        public async Task CreateSimpleSearchOperationAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var statusCode = HttpStatusCode.Unauthorized;
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };

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

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.CreateSimpleSearchOperationAsync(repoId, request: request));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/SimpleSearches");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post  // we expected a POST request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }
    }
}
