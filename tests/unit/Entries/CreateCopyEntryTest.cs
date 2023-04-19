using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class CreateCopyEntryTest
    {
        [Fact]
        public async Task CreateOrCopyEntryAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = "entry1"
            };
            var entry = new Folder()
            {
                Id = 100,
                Name = "entry1",
                ParentId = 1,
                FullPath = @"\entry1",
                FolderPath = @"\",
                Creator = "",
                CreationTime = DateTimeOffset.Parse("2021-01-01"),
                LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                EntryType = EntryType.Folder,
                TemplateName = null,
                TemplateId = 0,
                VolumeName = "default",
                RowNumber = 1
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
                   Content = new StringContent(JsonConvert.SerializeObject(entry))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.CreateOrCopyEntryAsync(repoId, (int)entry.ParentId, request).ConfigureAwait(false);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.Equal(entry.Name, result.Name);
            Assert.Equal(entry.ParentId, result.ParentId);
            Assert.Equal(entry.FullPath, result.FullPath);
            Assert.Equal(entry.FolderPath, result.FolderPath);
            Assert.Equal(entry.Creator, result.Creator);
            Assert.Equal(entry.CreationTime, result.CreationTime);
            Assert.Equal(entry.LastModifiedTime, result.LastModifiedTime);
            Assert.Equal(entry.EntryType, result.EntryType);
            Assert.Equal(entry.TemplateName, result.TemplateName);
            Assert.Equal(entry.TemplateId, result.TemplateId);
            Assert.Equal(entry.VolumeName, result.VolumeName);
            Assert.Equal(entry.RowNumber, result.RowNumber);
            Assert.Equal(typeof(Folder), result.GetType());

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{(int)entry.ParentId}/Laserfiche.Repository.Folder/children");

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
        public async Task CreateOrCopyEntryAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 100;
            var statusCode = HttpStatusCode.Unauthorized;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = "entry1"
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.CreateOrCopyEntryAsync(repoId, entryId, request).ConfigureAwait(false)).ConfigureAwait(false);

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/Laserfiche.Repository.Folder/children");

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
        public async Task CreateOrCopyEntryAsync_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = "entry1"
            };
            var entry = new Folder()
            {
                Id = 100,
                Name = "entry1",
                ParentId = 1,
                FullPath = @"\entry1",
                FolderPath = @"\",
                Creator = "",
                CreationTime = DateTimeOffset.Parse("2021-01-01"),
                LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                EntryType = EntryType.Folder,
                TemplateName = null,
                TemplateId = 0,
                VolumeName = "default",
                RowNumber = 1
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
                   Content = new StringContent(JsonConvert.SerializeObject(entry))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.CreateOrCopyEntryAsync(repoId, (int)entry.ParentId, request, autoRename: true).ConfigureAwait(false);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.Equal(entry.Name, result.Name);
            Assert.Equal(entry.ParentId, result.ParentId);
            Assert.Equal(entry.FullPath, result.FullPath);
            Assert.Equal(entry.FolderPath, result.FolderPath);
            Assert.Equal(entry.Creator, result.Creator);
            Assert.Equal(entry.CreationTime, result.CreationTime);
            Assert.Equal(entry.LastModifiedTime, result.LastModifiedTime);
            Assert.Equal(entry.EntryType, result.EntryType);
            Assert.Equal(entry.TemplateName, result.TemplateName);
            Assert.Equal(entry.TemplateId, result.TemplateId);
            Assert.Equal(entry.VolumeName, result.VolumeName);
            Assert.Equal(entry.RowNumber, result.RowNumber);
            Assert.Equal(typeof(Folder), result.GetType());

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{(int)entry.ParentId}/Laserfiche.Repository.Folder/children?autoRename=true");

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
