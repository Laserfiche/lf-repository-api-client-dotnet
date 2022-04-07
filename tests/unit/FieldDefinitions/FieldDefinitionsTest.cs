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

namespace Laserfiche.Repository.Api.Client.Test.FieldDefinitions
{
    public class FieldDefinitionsTest
    {
        [Fact]
        public async Task GetFieldDefinitionsAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

            WFieldInfo fieldInfo1 = new WFieldInfo()
            {
                Name = "name1",
                Id = 1,
                Description = "description1",
                FieldType = WFieldType.Blob,
                Length = 100,
                DefaultValue = "default1",
                IsMultiValue = false,
                IsRequired = false,
                Constraint = "constraint1",
                ConstraintError = "error1",
                ListValues = new List<string>() { "value1" },
                Format = WFieldFormat.Custom,
                Currency = "currency1",
                FormatPattern = "format1"
            };
            WFieldInfo fieldInfo2 = new WFieldInfo()
            {
                Name = "name2",
                Id = 2,
                Description = "description2",
                FieldType = WFieldType.Date,
                Length = 200,
                DefaultValue = "default2",
                IsMultiValue = true,
                IsRequired = true,
                Constraint = "constraint2",
                ConstraintError = "error2",
                ListValues = new List<string>() { "value2" },
                Format = WFieldFormat.LongDate,
                Currency = "currency2",
                FormatPattern = "format2"
            };
            ODataValueOfIListOfWFieldInfo ret = new ODataValueOfIListOfWFieldInfo()
            {
                Value = new List<WFieldInfo>() { fieldInfo1, fieldInfo2 }
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

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.GetFieldDefinitionsAsync(repoId);
            var result = swaggerResponse.Result.Value;
            Assert.Equal(2, result.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(ret.Value.ElementAt(i).Name, result.ElementAt(i).Name);
                Assert.Equal(ret.Value.ElementAt(i).Id, result.ElementAt(i).Id);
                Assert.Equal(ret.Value.ElementAt(i).Description, result.ElementAt(i).Description);
                Assert.Equal(ret.Value.ElementAt(i).FieldType, result.ElementAt(i).FieldType);
                Assert.Equal(ret.Value.ElementAt(i).DefaultValue, result.ElementAt(i).DefaultValue);
                Assert.Equal(ret.Value.ElementAt(i).IsMultiValue, result.ElementAt(i).IsMultiValue);
                Assert.Equal(ret.Value.ElementAt(i).IsRequired, result.ElementAt(i).IsRequired);
                Assert.Equal(ret.Value.ElementAt(i).Constraint, result.ElementAt(i).Constraint);
                Assert.Equal(ret.Value.ElementAt(i).ConstraintError, result.ElementAt(i).ConstraintError);
                Assert.Equal(ret.Value.ElementAt(i).Format, result.ElementAt(i).Format);
                Assert.Equal(ret.Value.ElementAt(i).Currency, result.ElementAt(i).Currency);
                Assert.Equal(ret.Value.ElementAt(i).FormatPattern, result.ElementAt(i).FormatPattern);
                Assert.Equal(ret.Value.ElementAt(i).ListValues.ElementAt(0), result.ElementAt(i).ListValues.ElementAt(0));
            }

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/FieldDefinitions");

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
        public async Task GetFieldDefinitionsAsync_ODataQueryOptions()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";

            WFieldInfo fieldInfo1 = new WFieldInfo()
            {
                Name = "name1",
                Id = 1,
                Description = "description1",
                FieldType = WFieldType.Blob,
                Length = 100,
                DefaultValue = "default1",
                IsMultiValue = false,
                IsRequired = false,
                Constraint = "constraint1",
                ConstraintError = "error1",
                ListValues = new List<string>() { "value1" },
                Format = WFieldFormat.Custom,
                Currency = "currency1",
                FormatPattern = "format1"
            };
            WFieldInfo fieldInfo2 = new WFieldInfo()
            {
                Name = "name2",
                Id = 2,
                Description = "description2",
                FieldType = WFieldType.Date,
                Length = 200,
                DefaultValue = "default2",
                IsMultiValue = true,
                IsRequired = true,
                Constraint = "constraint2",
                ConstraintError = "error2",
                ListValues = new List<string>() { "value2" },
                Format = WFieldFormat.LongDate,
                Currency = "currency2",
                FormatPattern = "format2"
            };
            ODataValueOfIListOfWFieldInfo ret = new ODataValueOfIListOfWFieldInfo()
            {
                Value = new List<WFieldInfo>() { fieldInfo1, fieldInfo2 }
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

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            _ = await client.GetFieldDefinitionsAsync(repoId, prefer:"Prefer", select:"select", orderby:"orderby", top:1, skip:2, count:true);

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/FieldDefinitions?%24select=select&%24orderby=orderby&%24top=1&%24skip=2&%24count=true");

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
        public async Task GetFieldDefinitionsAsync_AnyOther()
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

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.GetFieldDefinitionsAsync(repoId));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://api.laserfiche.com/v1/Repositories/repoId/FieldDefinitions");

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
        public async Task GetFieldDefinitionByIdAsync_200()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int id = 1;

            WFieldInfo fieldInfo1 = new WFieldInfo()
            {
                Name = "name1",
                Id = id,
                Description = "description1",
                FieldType = WFieldType.Blob,
                Length = 100,
                DefaultValue = "default1",
                IsMultiValue = false,
                IsRequired = false,
                Constraint = "constraint1",
                ConstraintError = "error1",
                ListValues = new List<string>() { "value1" },
                Format = WFieldFormat.Custom,
                Currency = "currency1",
                FormatPattern = "format1"
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
                    Content = new StringContent(JsonConvert.SerializeObject(fieldInfo1))
                })
                .Verifiable();


            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new LaserficheRepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.GetFieldDefinitionByIdAsync(repoId, fieldDefinitionId:id);
            var result = swaggerResponse.Result;
            Assert.Equal(result.Name, fieldInfo1.Name);
            Assert.Equal(result.Id, fieldInfo1.Id);
            Assert.Equal(result.Description, fieldInfo1.Description);
            Assert.Equal(result.FieldType, fieldInfo1.FieldType);
            Assert.Equal(result.DefaultValue, fieldInfo1.DefaultValue);
            Assert.Equal(result.IsMultiValue, fieldInfo1.IsMultiValue);
            Assert.Equal(result.IsRequired, fieldInfo1.IsRequired);
            Assert.Equal(result.Constraint, fieldInfo1.Constraint);
            Assert.Equal(result.ConstraintError, fieldInfo1.ConstraintError);
            Assert.Equal(result.Format, fieldInfo1.Format);
            Assert.Equal(result.Currency, fieldInfo1.Currency);
            Assert.Equal(result.FormatPattern, fieldInfo1.FormatPattern);
            Assert.Equal(result.ListValues.ElementAt(0), fieldInfo1.ListValues.ElementAt(0));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/FieldDefinitions/{id}");

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
        public async Task GetFieldDefinitionByIdAsync_AnyOther()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            var id = 1;

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
            await Assert.ThrowsAsync<ApiException>(async () => await client.GetFieldDefinitionByIdAsync(repoId, fieldDefinitionId:id));

            // ASSERT
            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/FieldDefinitions/{id}");

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
