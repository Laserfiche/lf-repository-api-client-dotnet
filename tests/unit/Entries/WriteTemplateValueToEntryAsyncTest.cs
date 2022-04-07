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
    public class WriteTemplateValueToEntryAsyncTest
    {
        [Fact]
        public async Task WriteTemplateValueToEntryAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var entryId = 1;
            var entry = new Entry()
            {
                Id = 1,
                Name = "entry1",
                ParentId = 1,
                FullPath = @"\entry1",
                FolderPath = @"\",
                Creator = "",
                CreationTime = DateTimeOffset.Parse("2021-01-01"),
                LastModifiedTime = DateTimeOffset.Parse("2021-01-01"),
                TemplateName = "templateName1",
                TemplateId = 1,
                VolumeName = "default",
                RowNumber = 1,
                EntryType = EntryType.Document,
                TemplateFieldNames = new List<string>() { "templateField" }
            };

            PutTemplateRequest request = new PutTemplateRequest()
            {
                TemplateName = "templateName",
                Fields = new Dictionary<string, FieldToUpdate>()
                {
                    ["templateField"] = new FieldToUpdate()
                    {
                        Values = new List<ValueToUpdate>() { new ValueToUpdate()
                            {
                                Value = "value",
                                Position = 1
                            }
                        }
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
                   Content = new StringContent(JsonConvert.SerializeObject(entry))
               })
               .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.WriteTemplateValueToEntryAsync(repoId, entryId, request:request);

            // ASSERT
            Assert.NotNull(swaggerResponse);
            Assert.Equal(200, swaggerResponse.StatusCode);
            Assert.NotNull(swaggerResponse.Result.TemplateFieldNames);
            Assert.Equal(1, swaggerResponse.Result.TemplateFieldNames.Count);
            Assert.Equal("templateField", swaggerResponse.Result.TemplateFieldNames.ElementAt(0));

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/template");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put// we expected a GET request
                    && req.RequestUri == expectedUri // to this uri
                    && req.Content.ToString() == new StringContent(JsonConvert.SerializeObject(request)).ToString() 
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task WriteTemplateValueToEntryAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var entryId = 1;
            
            PutTemplateRequest request = new PutTemplateRequest()
            {
                TemplateName = "templateName",
                Fields = new Dictionary<string, FieldToUpdate>()
                {
                    ["templateField"] = new FieldToUpdate()
                    {
                        Values = new List<ValueToUpdate>() { new ValueToUpdate()
                            {
                                Value = "value",
                                Position = 1
                            }
                        }
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
                   StatusCode = HttpStatusCode.InternalServerError
               })
               .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.WriteTemplateValueToEntryAsync(repoId, entryId, request:request));

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/template");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put// we expected a GET request
                    && req.RequestUri == expectedUri // to this uri
                    && req.Content.ToString() == new StringContent(JsonConvert.SerializeObject(request)).ToString()
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
