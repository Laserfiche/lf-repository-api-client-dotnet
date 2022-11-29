using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Entries
{
    public class ImportDocumentTest
    {
        [Fact]
        public async Task ImportDocumentAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int parentEntryId = 1;
            string fileName = "importdocument";
            string fileContent = "importdocument content";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                var electronicDocument = new FileParameter(stream, fileName, "application/pdf");
                var request = new PostEntryWithEdocMetadataRequest()
                {
                    Template = "template",
                    Metadata = new PutFieldValsRequest()
                    {
                        Fields = new Dictionary<string, FieldToUpdate>()
                        {
                            ["field1"] = new FieldToUpdate()
                            {
                                Values = new List<ValueToUpdate>()
                                {
                                    new ValueToUpdate() { Value = "new value", Position = 1 },
                                    new ValueToUpdate() { Value = "second value", Position = 2 }
                                }
                            },
                            ["field2"] = new FieldToUpdate()
                            {
                                Values = new List<ValueToUpdate>()
                                {
                                    new ValueToUpdate() { Value = "new value for field 2", Position = 1 },
                                }
                            }
                        },
                        Tags = new List<string>() { "tag1", "tag2" },
                        Links = new List<LinkToUpdate>()
                        {
                            new LinkToUpdate()
                            {
                                LinkTypeId = 1,
                                OtherSourceId = 123,
                                IsSource = true
                            }
                        }
                    }
                };
                var createEntryResult = new CreateEntryResult()
                {
                    Operations = new CreateEntryOperations()
                    {
                        EntryCreate = new EntryCreate()
                        {
                            EntryId = 100,
                            Exceptions = new List<APIServerException>()
                        },
                        SetEdoc = new SetEdoc()
                        {
                            Exceptions = new List<APIServerException>()
                        },
                        SetTemplate = new SetTemplate()
                        {
                            Template = request.Template,
                            Exceptions = new List<APIServerException>()
                        },
                        SetFields = new SetFields()
                        {
                            FieldCount = request.Metadata.Fields.Count,
                            Exceptions = new List<APIServerException>()
                        },
                        SetTags = new SetTags()
                        {
                            AssignedTags = request.Metadata.Tags,
                            Exceptions = new List<APIServerException>()
                        },
                        SetLinks = new SetLinks()
                        {
                            OtherEntryIds = request.Metadata.Links.Select(linkToUpdate => linkToUpdate.OtherSourceId).ToList(),
                            Exceptions = new List<APIServerException>()
                        }
                    },
                    DocumentLink = "link to created document"
                };

                string requestBodyContent = "";
                string requestFileContent = "";
                string requestFileContentType = "";
                string requestFileName = "";
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
                   .Callback<HttpRequestMessage, CancellationToken>((httpRequestMessage, _) =>
                   {
                       if (httpRequestMessage.Content is MultipartFormDataContent multipartFormDataContent)
                       {
                           foreach(var formContent in multipartFormDataContent)
                           {
                               if (formContent is StringContent stringContent)
                               {
                                   requestBodyContent = stringContent.ReadAsStringAsync().Result;
                               }
                               else if (formContent is StreamContent streamContent)
                               {
                                   requestFileContent = streamContent.ReadAsStringAsync().Result;
                                   requestFileContentType = streamContent.Headers.ContentType.MediaType;
                                   requestFileName = streamContent.Headers.ContentDisposition.FileName;
                               }
                           }
                       }
                   })
                   // prepare the expected response of the mocked http call
                   .ReturnsAsync(new HttpResponseMessage()
                   {
                       StatusCode = HttpStatusCode.Created,
                       Content = new StringContent(JsonConvert.SerializeObject(createEntryResult))
                   })
                   .Verifiable();

                // use real http client with mocked handler here
                var httpClient = new HttpClient(handlerMock.Object)
                {
                    BaseAddress = new Uri(baseAddress),
                };

                var client = new RepositoryApiClient(httpClient);

                // ACT
                var result = await client.EntriesClient.ImportDocumentAsync(repoId, parentEntryId, fileName, electronicDocument: electronicDocument, request: request);

                // ASSERT
                Assert.NotNull(result);
                Assert.NotNull(result.Operations);
                Assert.Equal(createEntryResult.Operations.EntryCreate.EntryId, result.Operations.EntryCreate.EntryId);
                Assert.Equal(createEntryResult.Operations.EntryCreate.Exceptions.Count, result.Operations.EntryCreate.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetEdoc.Exceptions.Count, result.Operations.SetEdoc.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetTemplate.Template, result.Operations.SetTemplate.Template);
                Assert.Equal(createEntryResult.Operations.SetTemplate.Exceptions.Count, result.Operations.SetTemplate.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetFields.FieldCount, result.Operations.SetFields.FieldCount);
                Assert.Equal(createEntryResult.Operations.SetFields.Exceptions.Count, result.Operations.SetFields.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetTags.AssignedTags, result.Operations.SetTags.AssignedTags);
                Assert.Equal(createEntryResult.Operations.SetTags.Exceptions.Count, result.Operations.SetTags.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetLinks.OtherEntryIds, result.Operations.SetLinks.OtherEntryIds);
                Assert.Equal(createEntryResult.Operations.SetLinks.Exceptions.Count, result.Operations.SetLinks.Exceptions.Count);
                Assert.Equal(createEntryResult.DocumentLink, result.DocumentLink);

                // also check the 'http' call was like we expected it
                var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{parentEntryId}/{fileName}");

                handlerMock.Protected().Verify(
                   "SendAsync",
                   Times.Exactly(1), // we expected a single external request
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Post  // we expected a POST request
                      && req.RequestUri == expectedUri // to this uri
                   ),
                   ItExpr.IsAny<CancellationToken>()
                );
                Assert.Equal(JsonConvert.SerializeObject(request), requestBodyContent);
                Assert.Equal(fileContent, requestFileContent);
                Assert.Equal(electronicDocument.ContentType, requestFileContentType);
                Assert.Equal(electronicDocument.FileName, requestFileName);
            }
        }

        [Fact]
        public async Task ImportDocumentAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int parentEntryId = 1;
            string fileName = "importdocument";
            string fileContent = "importdocument content";
            var statusCode = HttpStatusCode.Unauthorized;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                var electronicDocument = new FileParameter(stream, fileName, "application/pdf");
                var request = new PostEntryWithEdocMetadataRequest();

                string requestBodyContent = "";
                string requestFileContent = "";
                string requestFileContentType = "";
                string requestFileName = "";
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
                   .Callback<HttpRequestMessage, CancellationToken>((httpRequestMessage, _) =>
                   {
                       if (httpRequestMessage.Content is MultipartFormDataContent multipartFormDataContent)
                       {
                           foreach (var formContent in multipartFormDataContent)
                           {
                               if (formContent is StringContent stringContent)
                               {
                                   requestBodyContent = stringContent.ReadAsStringAsync().Result;
                               }
                               else if (formContent is StreamContent streamContent)
                               {
                                   requestFileContent = streamContent.ReadAsStringAsync().Result;
                                   requestFileContentType = streamContent.Headers.ContentType.MediaType;
                                   requestFileName = streamContent.Headers.ContentDisposition.FileName;
                               }
                           }
                       }
                   })
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
                var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.ImportDocumentAsync(repoId, parentEntryId, fileName, electronicDocument: electronicDocument, request: request));

                // ASSERT
                Assert.Equal((int)statusCode, response.StatusCode);

                // also check the 'http' call was like we expected it
                var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{parentEntryId}/{fileName}");

                handlerMock.Protected().Verify(
                   "SendAsync",
                   Times.Exactly(1), // we expected a single external request
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Post  // we expected a POST request
                      && req.RequestUri == expectedUri // to this uri
                   ),
                   ItExpr.IsAny<CancellationToken>()
                );
                Assert.Equal(JsonConvert.SerializeObject(request), requestBodyContent);
                Assert.Equal(fileContent, requestFileContent);
                Assert.Equal(electronicDocument.ContentType, requestFileContentType);
                Assert.Equal(electronicDocument.FileName, requestFileName);
            }
        }

        [Fact]
        public async Task ImportDocumentAsync_WithQueryParameter_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int parentEntryId = 1;
            string fileName = "importdocument";
            string fileContent = "importdocument content";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                var electronicDocument = new FileParameter(stream, fileName, "application/pdf");
                var request = new PostEntryWithEdocMetadataRequest()
                {
                    Template = "template",
                    Metadata = new PutFieldValsRequest()
                    {
                        Fields = new Dictionary<string, FieldToUpdate>()
                        {
                            ["field1"] = new FieldToUpdate()
                            {
                                Values = new List<ValueToUpdate>()
                            {
                                new ValueToUpdate() { Value = "new value", Position = 1 },
                                new ValueToUpdate() { Value = "second value", Position = 2 }
                            }
                            },
                            ["field2"] = new FieldToUpdate()
                            {
                                Values = new List<ValueToUpdate>()
                            {
                                new ValueToUpdate() { Value = "new value for field 2", Position = 1 },
                            }
                            }
                        },
                        Tags = new List<string>() { "tag1", "tag2" },
                        Links = new List<LinkToUpdate>()
                        {
                            new LinkToUpdate()
                            {
                                LinkTypeId = 1,
                                OtherSourceId = 123,
                                IsSource = true
                            }
                        }
                    }
                };
                var createEntryResult = new CreateEntryResult()
                {
                    Operations = new CreateEntryOperations()
                    {
                        EntryCreate = new EntryCreate()
                        {
                            EntryId = 100,
                            Exceptions = new List<APIServerException>()
                        },
                        SetEdoc = new SetEdoc()
                        {
                            Exceptions = new List<APIServerException>()
                        },
                        SetTemplate = new SetTemplate()
                        {
                            Template = request.Template,
                            Exceptions = new List<APIServerException>()
                        },
                        SetFields = new SetFields()
                        {
                            FieldCount = request.Metadata.Fields.Count,
                            Exceptions = new List<APIServerException>()
                        },
                        SetTags = new SetTags()
                        {
                            AssignedTags = request.Metadata.Tags,
                            Exceptions = new List<APIServerException>()
                        },
                        SetLinks = new SetLinks()
                        {
                            OtherEntryIds = request.Metadata.Links.Select(linkToUpdate => linkToUpdate.OtherSourceId).ToList(),
                            Exceptions = new List<APIServerException>()
                        }
                    },
                    DocumentLink = "link to created document"
                };

                string requestBodyContent = "";
                string requestFileContent = "";
                string requestFileContentType = "";
                string requestFileName = "";
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
                    .Callback<HttpRequestMessage, CancellationToken>((httpRequestMessage, _) =>
                    {
                        if (httpRequestMessage.Content is MultipartFormDataContent multipartFormDataContent)
                        {
                            foreach (var formContent in multipartFormDataContent)
                            {
                                if (formContent is StringContent stringContent)
                                {
                                    requestBodyContent = stringContent.ReadAsStringAsync().Result;
                                }
                                else if (formContent is StreamContent streamContent)
                                {
                                    requestFileContent = streamContent.ReadAsStringAsync().Result;
                                    requestFileContentType = streamContent.Headers.ContentType.MediaType;
                                    requestFileName = streamContent.Headers.ContentDisposition.FileName;
                                }
                            }
                        }
                    })
                    // prepare the expected response of the mocked http call
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Created,
                        Content = new StringContent(JsonConvert.SerializeObject(createEntryResult))
                    })
                    .Verifiable();

                // use real http client with mocked handler here
                var httpClient = new HttpClient(handlerMock.Object)
                {
                    BaseAddress = new Uri(baseAddress),
                };

                var client = new RepositoryApiClient(httpClient);

                // ACT
                var result = await client.EntriesClient.ImportDocumentAsync(repoId, parentEntryId, fileName, autoRename: true, electronicDocument: electronicDocument, request: request);

                // ASSERT
                Assert.NotNull(result);
                Assert.NotNull(result.Operations);
                Assert.Equal(createEntryResult.Operations.EntryCreate.EntryId, result.Operations.EntryCreate.EntryId);
                Assert.Equal(createEntryResult.Operations.EntryCreate.Exceptions.Count, result.Operations.EntryCreate.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetEdoc.Exceptions.Count, result.Operations.SetEdoc.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetTemplate.Template, result.Operations.SetTemplate.Template);
                Assert.Equal(createEntryResult.Operations.SetTemplate.Exceptions.Count, result.Operations.SetTemplate.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetFields.FieldCount, result.Operations.SetFields.FieldCount);
                Assert.Equal(createEntryResult.Operations.SetFields.Exceptions.Count, result.Operations.SetFields.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetTags.AssignedTags, result.Operations.SetTags.AssignedTags);
                Assert.Equal(createEntryResult.Operations.SetTags.Exceptions.Count, result.Operations.SetTags.Exceptions.Count);
                Assert.Equal(createEntryResult.Operations.SetLinks.OtherEntryIds, result.Operations.SetLinks.OtherEntryIds);
                Assert.Equal(createEntryResult.Operations.SetLinks.Exceptions.Count, result.Operations.SetLinks.Exceptions.Count);
                Assert.Equal(createEntryResult.DocumentLink, result.DocumentLink);

                // also check the 'http' call was like we expected it
                var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{parentEntryId}/{fileName}?autoRename=true");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1), // we expected a single external request
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post  // we expected a POST request
                        && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
                Assert.Equal(JsonConvert.SerializeObject(request), requestBodyContent);
                Assert.Equal(fileContent, requestFileContent);
                Assert.Equal(electronicDocument.ContentType, requestFileContentType);
                Assert.Equal(electronicDocument.FileName, requestFileName);
            }
        }
    }
}
