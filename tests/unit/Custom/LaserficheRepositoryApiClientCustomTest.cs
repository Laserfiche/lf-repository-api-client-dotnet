using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class LaserficheRepositoryApiClientCustomTest
    {
        #region GetEntryAsync with url
        [Fact]
        public void GetEntryAsync_InvalidEntryId()
        {
            string uriString = "http://host.laserfiche.com/wrongversion/Repositories/repo123/Entries/abc?$select=1";

            var client = new RepositoryApiClient(null);

            Assert.ThrowsAsync<ArgumentException>(() => client.EntriesClient.GetEntryAsync(uriString));
        }

        [Fact]
        public void GetEntryAsync_InvalidUri()
        {
            string uriString = "http://host.laserfiche.com/wrongversion/Repositories/repo123/Entries/456?$select=1";

            var client = new RepositoryApiClient(null);

            Assert.ThrowsAsync<ArgumentException>(() => client.EntriesClient.GetEntryAsync(uriString));
        }

        [Fact]
        public async Task GetEntryAsync_Return200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            Entry entry = new Entry()
            {
                Id = 10,
                Name = "EntryName",
                ParentId = 1,
                FullPath = "/EntryName",
                FolderPath = "/EntryName",
                Creator = "Creator",
                CreationTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EntryType = EntryType.Folder,
                TemplateName = "Template",
                TemplateId = 2,
                VolumeName = "Volume"
            };
            string uriString = $"{baseAddress}v1/Repositories/{repoId}/Entries/{entry.Id}?$select=1";

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
                   Content = new StringContent(JsonConvert.SerializeObject(entry)),
               })
               .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.GetEntryAsync(uriString);

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
            Assert.Equal(typeof(Folder), result.GetType());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && HttpUtility.UrlDecode(req.RequestUri.ToString()).Equals(uriString) // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
        #endregion

        #region SetDefaultHeaders

        [Fact]
        public async Task SetDefaultHeaders_DefaultHeadersInRequest()
        {
            // ARRANGE
            string acceptLanguageHeaderValue = "fr-FR";
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            Entry entry = new Entry()
            {
                Id = 10,
                Name = "EntryName",
                ParentId = 1,
                FullPath = "/EntryName",
                FolderPath = "/EntryName",
                Creator = "Creator",
                CreationTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EntryType = EntryType.Folder,
                TemplateName = "Template",
                TemplateId = 2,
                VolumeName = "Volume"
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
                   Content = new StringContent(JsonConvert.SerializeObject(entry)),
               })
               .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguageHeaderValue));

            // ACT
            var result = await client.EntriesClient.GetEntryAsync(repoId, entry.Id);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.Equal(entry.Name, result.Name);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entry.Id}");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  // we expected a GET request
                  && req.RequestUri == expectedUri // to this uri
                  && req.Headers.AcceptLanguage.ToString() == acceptLanguageHeaderValue
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        #endregion
    }
}
