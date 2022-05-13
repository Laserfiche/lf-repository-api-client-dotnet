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
    public class FieldDefinitionsByIdTest
    {
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

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var swaggerResponse = await client.FieldDefinitionsClient.GetFieldDefinitionByIdAsync(repoId, fieldDefinitionId: id);
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

            var client = new RepositoryApiClient(httpClient);

            // ACT
            await Assert.ThrowsAsync<ApiException>(async () => await client.FieldDefinitionsClient.GetFieldDefinitionByIdAsync(repoId, fieldDefinitionId: id));

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
