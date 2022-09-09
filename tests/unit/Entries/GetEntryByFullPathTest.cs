﻿using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class GetEntryByFullPathTest
    {
        [Fact]
        public async Task GetEntryByPathAsync_ReturnSuccessfulEntryStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string rootPath = @"\";
            bool fallBackToClosestAncestor = true;
            FindEntryResult getEntryByPath = new FindEntryResult()
            {
                Entry = new Entry()
                {
                    Id = 1,
                    Name = "EntryName",
                    ParentId = 1,
                    FullPath = @"\",
                    FolderPath = null,
                    Creator = "",
                    CreationTime = DateTimeOffset.Now,
                    LastModifiedTime = DateTimeOffset.Now,
                    EntryType = EntryType.Folder,
                    TemplateName = "Template",
                    TemplateId = 2,
                    VolumeName = "Volume"
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
                   Content = new StringContent(JsonConvert.SerializeObject(getEntryByPath))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetEntryByPathAsync(repoId, rootPath, fallBackToClosestAncestor);

            // ASSERT   
            Assert.NotNull(result);
            Assert.Null(result.AncestorEntry);
            Assert.Equal(getEntryByPath.AncestorEntry, result.AncestorEntry);
            Assert.Equal(getEntryByPath.Entry.Id, result.Entry.Id);
            Assert.Equal(getEntryByPath.Entry.Name, result.Entry.Name);
            Assert.Equal(getEntryByPath.Entry.ParentId, result.Entry.ParentId);
            Assert.Equal(getEntryByPath.Entry.FullPath, result.Entry.FullPath);
            Assert.Equal(getEntryByPath.Entry.FolderPath, result.Entry.FolderPath);
            Assert.Equal(getEntryByPath.Entry.Creator, result.Entry.Creator);
            Assert.Equal(getEntryByPath.Entry.CreationTime, result.Entry.CreationTime);
            Assert.Equal(getEntryByPath.Entry.LastModifiedTime, result.Entry.LastModifiedTime);
            Assert.Equal(getEntryByPath.Entry.EntryType, result.Entry.EntryType);
            Assert.Equal(getEntryByPath.Entry.TemplateName, result.Entry.TemplateName);
            Assert.Equal(getEntryByPath.Entry.TemplateId, result.Entry.TemplateId);
            Assert.Equal(getEntryByPath.Entry.VolumeName, result.Entry.VolumeName);
            Assert.Equal(typeof(Folder), result.Entry.GetType());

            // also check the 'http' call was like we expected it
            string urlEncodedRootPath = HttpUtility.UrlEncode(rootPath).ToUpper();
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/repoId/Entries/ByPath?fullPath={urlEncodedRootPath}&fallbackToClosestAncestor=true");

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
        public async Task GetEntryByPathAsync_ReturnSuccessfulAncestorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string nonExistingPath = @"\NonExistingPath";
            bool fallBackToClosestAncestor = true;
            FindEntryResult getAncestorEntryByPath = new FindEntryResult()
            {
                Entry = new Entry()
                {
                    Id = 1,
                    Name = "EntryName",
                    ParentId = 1,
                    FullPath = @"\",
                    FolderPath = null,
                    Creator = "",
                    CreationTime = DateTimeOffset.Now,
                    LastModifiedTime = DateTimeOffset.Now,
                    EntryType = EntryType.Folder,
                    TemplateName = "Template",
                    TemplateId = 2,
                    VolumeName = "Volume"
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
                   Content = new StringContent(JsonConvert.SerializeObject(getAncestorEntryByPath))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetEntryByPathAsync(repoId, nonExistingPath, fallBackToClosestAncestor);

            // ASSERT   
            Assert.NotNull(result);
            Assert.Null(result.AncestorEntry);
            Assert.Equal(getAncestorEntryByPath.AncestorEntry, result.AncestorEntry);
            Assert.Equal(getAncestorEntryByPath.Entry.Id, result.Entry.Id);
            Assert.Equal(getAncestorEntryByPath.Entry.Name, result.Entry.Name);
            Assert.Equal(getAncestorEntryByPath.Entry.ParentId, result.Entry.ParentId);
            Assert.Equal(getAncestorEntryByPath.Entry.FullPath, result.Entry.FullPath);
            Assert.Equal(getAncestorEntryByPath.Entry.FolderPath, result.Entry.FolderPath);
            Assert.Equal(getAncestorEntryByPath.Entry.Creator, result.Entry.Creator);
            Assert.Equal(getAncestorEntryByPath.Entry.CreationTime, result.Entry.CreationTime);
            Assert.Equal(getAncestorEntryByPath.Entry.LastModifiedTime, result.Entry.LastModifiedTime);
            Assert.Equal(getAncestorEntryByPath.Entry.EntryType, result.Entry.EntryType);
            Assert.Equal(getAncestorEntryByPath.Entry.TemplateName, result.Entry.TemplateName);
            Assert.Equal(getAncestorEntryByPath.Entry.TemplateId, result.Entry.TemplateId);
            Assert.Equal(getAncestorEntryByPath.Entry.VolumeName, result.Entry.VolumeName);
            Assert.Equal(typeof(Folder), result.Entry.GetType());

            // also check the 'http' call was like we expected it
            string urlEncodedRootPath = HttpUtility.UrlEncode(nonExistingPath);
            urlEncodedRootPath = urlEncodedRootPath.Replace("c", "C");
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/repoId/Entries/ByPath?fullPath={urlEncodedRootPath}&fallbackToClosestAncestor=true");

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
        public async Task GetEntryByPathAsync_ReturnUnauthorizedCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string rootPath = @"\";
            bool fallBackToClosestAncestor = true;
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.GetEntryByPathAsync(repoId, rootPath, fallBackToClosestAncestor));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            string urlEncodedRootPath = HttpUtility.UrlEncode(rootPath).ToUpper();
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/repoId/Entries/ByPath?fullPath={urlEncodedRootPath}&fallbackToClosestAncestor=true");

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
        public async Task GetEntryByPathAsync_ReturnNotFoundCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string nonExistingPath = @"\NonExistingPath";
            bool fallBackToClosestAncestor = false;
            var statusCode = HttpStatusCode.NotFound;

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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.GetEntryByPathAsync(repoId, nonExistingPath, fallBackToClosestAncestor));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            string urlEncodedRootPath = HttpUtility.UrlEncode(nonExistingPath);
            urlEncodedRootPath = urlEncodedRootPath.Replace("c", "C");
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/repoId/Entries/ByPath?fullPath={urlEncodedRootPath}&fallbackToClosestAncestor=false");

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