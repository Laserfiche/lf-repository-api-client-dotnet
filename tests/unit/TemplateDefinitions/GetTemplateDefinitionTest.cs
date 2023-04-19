using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.TemplateDefinitions
{
    public class GetTemplateDefinitionTest
    {
        [Fact]
        public async Task GetTemplateDefinitionsAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            ODataValueOfIListOfWTemplateInfo templateDefinitionList = new ODataValueOfIListOfWTemplateInfo()
            {
                Value = new List<WTemplateInfo>()
                {
                    new WTemplateInfo()
                    {
                        Id = 1,
                        Name = "Template1",
                        Description = "First Template Description",
                        FieldCount = 2,
                        Color = new LFColor()
                        {
                            A = 255,
                            R = 255,
                            G = 255,
                            B = 255
                        }
                    },
                    new WTemplateInfo()
                    {
                        Id = 10,
                        Name = "Template2",
                        Description = "Second Template Description",
                        FieldCount = 5,
                        Color = new LFColor()
                        {
                            A = 100,
                            R = 200,
                            G = 100,
                            B = 200
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
                   Content = new StringContent(JsonConvert.SerializeObject(templateDefinitionList)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(repoId).ConfigureAwait(false);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(templateDefinitionList.Value.Count, result.Value.Count);
            var templateDefinitionListEnumerator = templateDefinitionList.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (templateDefinitionListEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(templateDefinitionListEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(templateDefinitionListEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(templateDefinitionListEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(templateDefinitionListEnumerator.Current.FieldCount, resultEnumerator.Current.FieldCount);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.A, resultEnumerator.Current.Color.A);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.R, resultEnumerator.Current.Color.R);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.G, resultEnumerator.Current.Color.G);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.B, resultEnumerator.Current.Color.B);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions");

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
        public async Task GetTemplateDefinitionsAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
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
                   StatusCode = statusCode,
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(repoId).ConfigureAwait(false)).ConfigureAwait(false);

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions");

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
        public async Task GetTemplateDefinitionsAsync_WithPreferHeader_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string preferHeaderValue = "maxpagesize=1";
            string selectQueryParameter = "description";
            string orderbyQueryParameter = "name";
            int topQueryParameter = 1;
            int skipQueryParameter = 1;
            bool countQueryParameter = true;
            ODataValueOfIListOfWTemplateInfo templateDefinitionList = new ODataValueOfIListOfWTemplateInfo()
            {
                Value = new List<WTemplateInfo>()
                {
                    new WTemplateInfo()
                    {
                        Id = 1,
                        Name = "Template1",
                        Description = "First Template Description",
                        FieldCount = 2,
                        Color = new LFColor()
                        {
                            A = 255,
                            R = 255,
                            G = 255,
                            B = 255
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
                   Content = new StringContent(JsonConvert.SerializeObject(templateDefinitionList)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(repoId, prefer: preferHeaderValue,
                select: selectQueryParameter, orderby: orderbyQueryParameter, top: topQueryParameter,
                skip: skipQueryParameter, count: countQueryParameter).ConfigureAwait(false);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(templateDefinitionList.Value.Count, result.Value.Count);
            var templateDefinitionListEnumerator = templateDefinitionList.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (templateDefinitionListEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(templateDefinitionListEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(templateDefinitionListEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(templateDefinitionListEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(templateDefinitionListEnumerator.Current.FieldCount, resultEnumerator.Current.FieldCount);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.A, resultEnumerator.Current.Color.A);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.R, resultEnumerator.Current.Color.R);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.G, resultEnumerator.Current.Color.G);
                Assert.Equal(templateDefinitionListEnumerator.Current.Color.B, resultEnumerator.Current.Color.B);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions?" +
                $"%24select={selectQueryParameter}&%24orderby={orderbyQueryParameter}&%24top={topQueryParameter}&" +
                $"%24skip={skipQueryParameter}&%24count={countQueryParameter.ToString().ToLower()}");

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
