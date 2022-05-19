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

namespace Laserfiche.Repository.Api.Client.Test.Searches
{
    public class GetSearchContextHitsTest
    {
        [Fact]
        public async Task GetSearchContextHitsAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            int rowNumber = 5;
            var contextHits = new ODataValueOfIListOfContextHit()
            {
                Value = new List<ContextHit>()
                {
                    new ContextHit()
                    {
                        HitNumber = 1,
                        HitType = HitType.Edoc,
                        IsAnnotationHit = false,
                        AnnotationId = 0,
                        PageNumber = 1,
                        PageOffset = 0,
                        Context = "context hit",
                        Highlight1Offset = 0,
                        Highlight1Length = 0,
                        Highlight2Offset = 0,
                        Highlight2Length = 0,
                        HitWidth= 0,
                        EdocHitCount = 10,
                        FieldHitCount = 0,
                        FieldName = ""
                    },
                    new ContextHit()
                    {
                        HitNumber = 2,
                        HitType = HitType.Edoc,
                        IsAnnotationHit = false,
                        AnnotationId = 0,
                        PageNumber = 1,
                        PageOffset = 30,
                        Context = "context hit",
                        Highlight1Offset = 0,
                        Highlight1Length = 0,
                        Highlight2Offset = 0,
                        Highlight2Length = 0,
                        HitWidth= 0,
                        EdocHitCount = 10,
                        FieldHitCount = 0,
                        FieldName = ""
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
                   Content = new StringContent(JsonConvert.SerializeObject(contextHits))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.SearchesClient.GetSearchContextHitsAsync(repoId, searchToken, rowNumber);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(contextHits.Value.Count, result.Value.Count);
            var contextHitsEnumerator = contextHits.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (contextHitsEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(contextHitsEnumerator.Current.HitNumber, resultEnumerator.Current.HitNumber);
                Assert.Equal(contextHitsEnumerator.Current.HitType, resultEnumerator.Current.HitType);
                Assert.Equal(contextHitsEnumerator.Current.IsAnnotationHit, resultEnumerator.Current.IsAnnotationHit);
                Assert.Equal(contextHitsEnumerator.Current.AnnotationId, resultEnumerator.Current.AnnotationId);
                Assert.Equal(contextHitsEnumerator.Current.PageNumber, resultEnumerator.Current.PageNumber);
                Assert.Equal(contextHitsEnumerator.Current.PageOffset, resultEnumerator.Current.PageOffset);
                Assert.Equal(contextHitsEnumerator.Current.Context, resultEnumerator.Current.Context);
                Assert.Equal(contextHitsEnumerator.Current.Highlight1Offset, resultEnumerator.Current.Highlight1Offset);
                Assert.Equal(contextHitsEnumerator.Current.Highlight1Length, resultEnumerator.Current.Highlight1Length);
                Assert.Equal(contextHitsEnumerator.Current.Highlight2Offset, resultEnumerator.Current.Highlight2Offset);
                Assert.Equal(contextHitsEnumerator.Current.Highlight2Length, resultEnumerator.Current.Highlight2Length);
                Assert.Equal(contextHitsEnumerator.Current.HitWidth, resultEnumerator.Current.HitWidth);
                Assert.Equal(contextHitsEnumerator.Current.EdocHitCount, resultEnumerator.Current.EdocHitCount);
                Assert.Equal(contextHitsEnumerator.Current.FieldHitCount, resultEnumerator.Current.FieldHitCount);
                Assert.Equal(contextHitsEnumerator.Current.FieldName, resultEnumerator.Current.FieldName);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}/Results/{rowNumber}/ContextHits");

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
        public async Task GetSearchContextHitsAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            int rowNumber = 5;
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.SearchesClient.GetSearchContextHitsAsync(repoId, searchToken, rowNumber));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}/Results/{rowNumber}/ContextHits");

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
        public async Task GetSearchContextHitsAsync_WithPreferHeader_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            string searchToken = "123-ABC";
            int rowNumber = 5;
            string preferHeaderValue = "maxpagesize=1";
            string selectQueryParameter = "hitNumber";
            string orderbyQueryParameter = "pageNumber";
            int topQueryParameter = 1;
            int skipQueryParameter = 1;
            bool countQueryParameter = true;
            var contextHits = new ODataValueOfIListOfContextHit()
            {
                Value = new List<ContextHit>()
                {
                    new ContextHit()
                    {
                        HitNumber = 1,
                        HitType = HitType.Edoc,
                        IsAnnotationHit = false,
                        AnnotationId = 0,
                        PageNumber = 1,
                        PageOffset = 0,
                        Context = "context hit",
                        Highlight1Offset = 0,
                        Highlight1Length = 0,
                        Highlight2Offset = 0,
                        Highlight2Length = 0,
                        HitWidth= 0,
                        EdocHitCount = 10,
                        FieldHitCount = 0,
                        FieldName = ""
                    },
                    new ContextHit()
                    {
                        HitNumber = 2,
                        HitType = HitType.Edoc,
                        IsAnnotationHit = false,
                        AnnotationId = 0,
                        PageNumber = 1,
                        PageOffset = 30,
                        Context = "context hit",
                        Highlight1Offset = 0,
                        Highlight1Length = 0,
                        Highlight2Offset = 0,
                        Highlight2Length = 0,
                        HitWidth= 0,
                        EdocHitCount = 10,
                        FieldHitCount = 0,
                        FieldName = ""
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
                   Content = new StringContent(JsonConvert.SerializeObject(contextHits))
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.SearchesClient.GetSearchContextHitsAsync(repoId, searchToken, rowNumber,
                prefer: preferHeaderValue, select: selectQueryParameter, orderby: orderbyQueryParameter,
                top: topQueryParameter, skip: skipQueryParameter, count: countQueryParameter);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(contextHits.Value.Count, result.Value.Count);
            var contextHitsEnumerator = contextHits.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (contextHitsEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(contextHitsEnumerator.Current.HitNumber, resultEnumerator.Current.HitNumber);
                Assert.Equal(contextHitsEnumerator.Current.HitType, resultEnumerator.Current.HitType);
                Assert.Equal(contextHitsEnumerator.Current.IsAnnotationHit, resultEnumerator.Current.IsAnnotationHit);
                Assert.Equal(contextHitsEnumerator.Current.AnnotationId, resultEnumerator.Current.AnnotationId);
                Assert.Equal(contextHitsEnumerator.Current.PageNumber, resultEnumerator.Current.PageNumber);
                Assert.Equal(contextHitsEnumerator.Current.PageOffset, resultEnumerator.Current.PageOffset);
                Assert.Equal(contextHitsEnumerator.Current.Context, resultEnumerator.Current.Context);
                Assert.Equal(contextHitsEnumerator.Current.Highlight1Offset, resultEnumerator.Current.Highlight1Offset);
                Assert.Equal(contextHitsEnumerator.Current.Highlight1Length, resultEnumerator.Current.Highlight1Length);
                Assert.Equal(contextHitsEnumerator.Current.Highlight2Offset, resultEnumerator.Current.Highlight2Offset);
                Assert.Equal(contextHitsEnumerator.Current.Highlight2Length, resultEnumerator.Current.Highlight2Length);
                Assert.Equal(contextHitsEnumerator.Current.HitWidth, resultEnumerator.Current.HitWidth);
                Assert.Equal(contextHitsEnumerator.Current.EdocHitCount, resultEnumerator.Current.EdocHitCount);
                Assert.Equal(contextHitsEnumerator.Current.FieldHitCount, resultEnumerator.Current.FieldHitCount);
                Assert.Equal(contextHitsEnumerator.Current.FieldName, resultEnumerator.Current.FieldName);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/Searches/{searchToken}/Results/{rowNumber}/ContextHits?" +
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
