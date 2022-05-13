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

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class GetEntryListingTest
    {
        [Fact]
        public async Task GetEntryListingAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            ODataValueContextOfIListOfODataGetEntryChildren entries = new ODataValueContextOfIListOfODataGetEntryChildren()
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
                   Content = new StringContent(JsonConvert.SerializeObject(entries))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.EntriesClient.GetEntryListingAsync(repoId, entryId);
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
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Folder/children");

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
        public async Task GetEntryListingAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.GetEntryListingAsync(repoId, entryId));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Folder/children");

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
        public async Task GetEntryListingAsync_WithPreferHeader_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            bool groupByEntryType = true;
            string preferHeaderValue = "maxpagesize=1";
            string selectQueryParameter = "name";
            string orderbyQueryParameter = "name";
            int topQueryParameter = 1;
            int skipQueryParameter = 1;
            bool countQueryParameter = true;
            bool formatFields = true;
            string culture = "fr";
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
                   Content = new StringContent(JsonConvert.SerializeObject(entries))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await client.EntriesClient.GetEntryListingAsync(repoId, entryId, groupByEntryType: groupByEntryType,
                fields: new List<string>() { "field1", "field2" }, formatFields: formatFields, prefer: preferHeaderValue, culture: culture, select: selectQueryParameter,
                orderby: orderbyQueryParameter,top: topQueryParameter, skip: skipQueryParameter, count: countQueryParameter);
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
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Folder/children?" +
                $"groupByEntryType={groupByEntryType.ToString().ToLower()}&fields=field1&fields=field2&formatFields={formatFields.ToString().ToLower()}" +
                $"&culture={culture}&%24select={selectQueryParameter}&%24orderby={orderbyQueryParameter}&" +
                $"%24top={topQueryParameter}&%24skip={skipQueryParameter}&%24count={countQueryParameter.ToString().ToLower()}");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get  // we expected a GET request
                    && req.RequestUri == expectedUri // to this uri
                    && req.Headers.Contains("Prefer") // and Prefer header with maxpagesize value
                    && req.Headers.GetValues("Prefer").FirstOrDefault() == preferHeaderValue
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
