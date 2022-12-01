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
    public class GetTemplateDefinitionFieldsTest
    {
        [Fact]
        public async Task GetTemplateFieldDefinitionsAsync_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int templateDefinitionId = 1;
            ODataValueOfIListOfTemplateFieldInfo templateFieldDefinitionList = new ODataValueOfIListOfTemplateFieldInfo()
            {
                Value = new List<TemplateFieldInfo>()
                {
                    new TemplateFieldInfo()
                    {
                        Id = 1,
                        Name = "Field1",
                        Description = "First Field Description",
                        FieldType = WFieldType.String,
                        Length = 100,
                        DefaultValue = null,
                        IsMultiValue = false,
                        IsRequired = false,
                        Constraint = "",
                        ConstraintError = "",
                        ListValues = new List<string>(),
                        Format = WFieldFormat.None,
                        Currency = "",
                        FormatPattern = "",
                        GroupId = 0,
                        GroupName = null,
                        Rule = null
                    },
                    new TemplateFieldInfo()
                    {
                        Id = 10,
                        Name = "Field2",
                        Description = "Second Field Description",
                        FieldType = WFieldType.DateTime,
                        Length = 0,
                        DefaultValue = null,
                        IsMultiValue = false,
                        IsRequired = false,
                        Constraint = "",
                        ConstraintError = "",
                        ListValues = new List<string>(),
                        Format = WFieldFormat.ShortDateTime,
                        Currency = "",
                        FormatPattern = "",
                        GroupId = 0,
                        GroupName = null,
                        Rule = null
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
                   Content = new StringContent(JsonConvert.SerializeObject(templateFieldDefinitionList)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(repoId, templateDefinitionId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(templateFieldDefinitionList.Value.Count, result.Value.Count);
            var templateFieldDefinitionListEnumerator = templateFieldDefinitionList.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (templateFieldDefinitionListEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.FieldType, resultEnumerator.Current.FieldType);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Length, resultEnumerator.Current.Length);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.DefaultValue, resultEnumerator.Current.DefaultValue);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.IsMultiValue, resultEnumerator.Current.IsMultiValue);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.IsRequired, resultEnumerator.Current.IsRequired);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Constraint, resultEnumerator.Current.Constraint);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.ConstraintError, resultEnumerator.Current.ConstraintError);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.ListValues.Count, resultEnumerator.Current.ListValues.Count);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Format, resultEnumerator.Current.Format);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Currency, resultEnumerator.Current.Currency);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.FormatPattern, resultEnumerator.Current.FormatPattern);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.GroupId, resultEnumerator.Current.GroupId);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.GroupName, resultEnumerator.Current.GroupName);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Rule, resultEnumerator.Current.Rule);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions/{templateDefinitionId}/Fields");

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
        public async Task GetTemplateFieldDefinitionsAsync_ReturnErrorStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int templateDefinitionId = 1;
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
            var response = await Assert.ThrowsAsync<ApiException>(async () => await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(repoId, templateDefinitionId));

            // ASSERT
            Assert.Equal((int)statusCode, response.StatusCode);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions/{templateDefinitionId}/Fields");

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
        public async Task GetTemplateFieldDefinitionsAsync_WithPreferHeader_WithQueryParameters_ReturnSuccessfulStatusCode()
        {
            // ARRANGE
            string baseAddress = "http://api.laserfiche.com/";
            string repoId = "repoId";
            int templateDefinitionId = 1;
            string preferHeaderValue = "maxpagesize=1";
            string selectQueryParameter = "description";
            string orderbyQueryParameter = "name";
            int topQueryParameter = 1;
            int skipQueryParameter = 1;
            bool countQueryParameter = true;
            ODataValueOfIListOfTemplateFieldInfo templateFieldDefinitionList = new ODataValueOfIListOfTemplateFieldInfo()
            {
                Value = new List<TemplateFieldInfo>()
                {
                    new TemplateFieldInfo()
                    {
                        Id = 1,
                        Name = "Field1",
                        Description = "First Field Description",
                        FieldType = WFieldType.String,
                        Length = 100,
                        DefaultValue = null,
                        IsMultiValue = false,
                        IsRequired = false,
                        Constraint = "",
                        ConstraintError = "",
                        ListValues = new List<string>(),
                        Format = WFieldFormat.None,
                        Currency = "",
                        FormatPattern = "",
                        GroupId = 0,
                        GroupName = null,
                        Rule = null
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
                   Content = new StringContent(JsonConvert.SerializeObject(templateFieldDefinitionList)),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var client = new RepositoryApiClient(httpClient);

            // ACT
            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(repoId, templateDefinitionId, prefer: preferHeaderValue,
                select: selectQueryParameter, orderby: orderbyQueryParameter, top: topQueryParameter,
                skip: skipQueryParameter, count: countQueryParameter);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(templateFieldDefinitionList.Value.Count, result.Value.Count);
            var templateFieldDefinitionListEnumerator = templateFieldDefinitionList.Value.GetEnumerator();
            var resultEnumerator = result.Value.GetEnumerator();
            while (templateFieldDefinitionListEnumerator.MoveNext() && resultEnumerator.MoveNext())
            {
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Id, resultEnumerator.Current.Id);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Name, resultEnumerator.Current.Name);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Description, resultEnumerator.Current.Description);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.FieldType, resultEnumerator.Current.FieldType);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Length, resultEnumerator.Current.Length);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.DefaultValue, resultEnumerator.Current.DefaultValue);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.IsMultiValue, resultEnumerator.Current.IsMultiValue);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.IsRequired, resultEnumerator.Current.IsRequired);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Constraint, resultEnumerator.Current.Constraint);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.ConstraintError, resultEnumerator.Current.ConstraintError);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.ListValues.Count, resultEnumerator.Current.ListValues.Count);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Format, resultEnumerator.Current.Format);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Currency, resultEnumerator.Current.Currency);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.FormatPattern, resultEnumerator.Current.FormatPattern);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.GroupId, resultEnumerator.Current.GroupId);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.GroupName, resultEnumerator.Current.GroupName);
                Assert.Equal(templateFieldDefinitionListEnumerator.Current.Rule, resultEnumerator.Current.Rule);
            }

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri(baseAddress + $"v1/Repositories/{repoId}/TemplateDefinitions/{templateDefinitionId}/Fields?" +
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
