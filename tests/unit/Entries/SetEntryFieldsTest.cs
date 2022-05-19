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
    public class SetEntryFieldsTest
    {
        [Fact]
        public async Task AssignFieldValuesAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            var request = new Dictionary<string, FieldToUpdate>()
            {
                ["Field1"] = new FieldToUpdate()
                {
                    Values = new List<ValueToUpdate>()
                    {
                        new ValueToUpdate() { Value = "field value 1", Position = 1 },
                    }
                },
                ["Field2"] = new FieldToUpdate()
                {
                    Values = new List<ValueToUpdate>()
                    {
                        new ValueToUpdate() { Value = "field value 2", Position = 1 },
                    }
                }
            };
            ODataValueOfIListOfFieldValue entryFields = new ODataValueOfIListOfFieldValue()
            {
                Value = new List<FieldValue>()
                {
                    new FieldValue()
                    {
                        FieldName = "Field1",
                        FieldType = WFieldType.String,
                        GroupId = 0,
                        FieldId = 1,
                        IsMultiValue = false,
                        IsRequired = false,
                        Values = new List<IDictionary<string, object>>()
                        {
                            new Dictionary<string, object>(){ ["value"] = "field value 1", ["position"] = 0 }
                        }
                    },
                    new FieldValue()
                    {
                        FieldName = "Field2",
                        FieldType = WFieldType.String,
                        GroupId = 0,
                        FieldId = 2,
                        IsMultiValue = false,
                        IsRequired = false,
                        Values = new List<IDictionary<string, object>>()
                        {
                            new Dictionary<string, object>(){ ["value"] = "field value 2", ["position"] = 0 }
                        }
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
                   Content = new StringContent(JsonConvert.SerializeObject(entryFields))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.EntriesClient.AssignFieldValuesAsync(repoId, entryId, fieldsToUpdate: request);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(entryFields.Value.Count, result.Value.Count);
            var fieldsEnumerator = entryFields.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (fieldsEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(fieldsEnumerator.Current.FieldName, resultEnumerator.Current.FieldName);
                Assert.Equal(fieldsEnumerator.Current.FieldType, resultEnumerator.Current.FieldType);
                Assert.Equal(fieldsEnumerator.Current.GroupId, resultEnumerator.Current.GroupId);
                Assert.Equal(fieldsEnumerator.Current.FieldId, resultEnumerator.Current.FieldId);
                Assert.Equal(fieldsEnumerator.Current.IsMultiValue, resultEnumerator.Current.IsMultiValue);
                Assert.Equal(fieldsEnumerator.Current.IsRequired, resultEnumerator.Current.IsRequired);
                Assert.Equal(fieldsEnumerator.Current.Values.Count, resultEnumerator.Current.Values.Count);
                Assert.Equal(fieldsEnumerator.Current.Values.First()["value"], resultEnumerator.Current.Values.First()["value"]);
                Assert.Equal(Convert.ToInt32(fieldsEnumerator.Current.Values.First()["position"]), Convert.ToInt32(resultEnumerator.Current.Values.First()["position"]));
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/fields");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put  // we expected a PUT request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }

        [Fact]
        public async Task AssignFieldValuesAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int entryId = 1;
            var statusCode = HttpStatusCode.Unauthorized;
            var request = new Dictionary<string, FieldToUpdate>()
            {
                ["Field1"] = new FieldToUpdate()
                {
                    Values = new List<ValueToUpdate>()
                    {
                        new ValueToUpdate() { Value = "field value 1", Position = 1 },
                    }
                },
                ["Field2"] = new FieldToUpdate()
                {
                    Values = new List<ValueToUpdate>()
                    {
                        new ValueToUpdate() { Value = "field value 2", Position = 1 },
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.EntriesClient.AssignFieldValuesAsync(repoId, entryId, fieldsToUpdate: request));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Entries/{entryId}/fields");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // we expected a single external request
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put  // we expected a PUT request
                  && req.RequestUri == expectedUri // to this uri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
            Assert.Equal(JsonConvert.SerializeObject(request), requestContent);
        }
    }
}
