using Laserfiche.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class ApiExceptionExtensionsTest
    {
        private void AssertApiExceptionAreEqual(ApiException expected, ApiException actual)
        {
            Assert.Equal(expected.StatusCode, actual.StatusCode);
            Assert.Equal(expected.Message, actual.Message);
            Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
            Assert.Equal(expected.Headers.Count, actual.Headers.Count);
            foreach (var header in expected.Headers.Keys)
            {
                Assert.Equal(expected.Headers[header], actual.Headers[header]);
            }

            Assert.Equal(expected.ProblemDetails.Title, actual.ProblemDetails.Title);
            Assert.Equal(expected.ProblemDetails.Type, actual.ProblemDetails.Type);
            Assert.Equal(expected.ProblemDetails.Status, actual.ProblemDetails.Status);
            Assert.Equal(expected.ProblemDetails.Detail, actual.ProblemDetails.Detail);
            Assert.Equal(expected.ProblemDetails.Instance, actual.ProblemDetails.Instance);
            Assert.Equal(expected.ProblemDetails.OperationId, actual.ProblemDetails.OperationId);
            Assert.Equal(expected.ProblemDetails.ErrorSource, actual.ProblemDetails.ErrorSource);
            Assert.Equal(expected.ProblemDetails.ErrorCode, actual.ProblemDetails.ErrorCode);
            Assert.Equal(expected.ProblemDetails.TraceId, actual.ProblemDetails.TraceId);
            Assert.Equal(expected.ProblemDetails.Extensions.Count, actual.ProblemDetails.Extensions.Count);
            foreach (var extension in expected.ProblemDetails.Extensions.Keys)
            {
                Assert.Equal(expected.ProblemDetails.Extensions[extension], actual.ProblemDetails.Extensions[extension]);
            }
        }

        [Fact]
        public void Create_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException expectedException = ApiException.Create(statusCode, headers, innerException);
            ApiException actualException = ApiExceptionExtensions.Create(statusCode, headers, innerException);

            AssertApiExceptionAreEqual(expectedException, actualException);
        }

        [Fact]
        public void Create_WithProblemDetails_ExceptionHasFullProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = "An error occurred.",
                Type = "ErrorType",
                Detail = "Detail",
                Status = statusCode,
                Instance = "Instance",
                OperationId = operationId,
                ErrorCode = 123,
                ErrorSource = "ErrorSource",
                TraceId = "TraceId",
                Extensions = new Dictionary<string, object>() { ["key"] = "value" }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException expectedException = ApiException.Create(statusCode, headers, problemDetails, innerException);
            ApiException actualException = ApiExceptionExtensions.Create(statusCode, headers, problemDetails, innerException);

            AssertApiExceptionAreEqual(expectedException, actualException);
        }

        [Fact]
        public void Create_WithResponseString_ResponseStringIsProblemDetails_ExceptionHasFullProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = "An error occurred.",
                Type = "ErrorType",
                Detail = "Detail",
                Status = statusCode,
                Instance = "Instance",
                OperationId = operationId,
                ErrorCode = 123,
                ErrorSource = "ErrorSource",
                TraceId = "TraceId",
                Extensions = new Dictionary<string, object>() { ["key"] = "value" }
            };
            string problemDetailsResponseString = JsonConvert.SerializeObject(problemDetails);
            Exception innerException = new Exception("An error occurred.");

            ApiException expectedException = ApiException.Create(statusCode, headers, problemDetailsResponseString, null, innerException);
            ApiException actualException = ApiExceptionExtensions.Create(statusCode, headers, problemDetailsResponseString, null, innerException);

            AssertApiExceptionAreEqual(expectedException, actualException);
        }

        [Fact]
        public void Create_WithCreateEntryResult_ExceptionHasMinimalProblemDetailsWithCreateEntryResult()
        {
            int statusCode = 400;
            string operationId = "123456789";
            int entryId = 123;
            string setTemplateErrorMessage = "Error setting template.";
            string setFieldsErrorMessage1 = "Error setting field1.";
            string setFieldsErrorMessage2 = "Error setting field2.";
            string setTagsErrorMessage = "Error setting tag.";
            string setLinksErrorMessage = "Error setting link.";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = new EntryCreate() { EntryId = entryId },
                    SetEdoc = new SetEdoc(),
                    SetTemplate = new SetTemplate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTemplateErrorMessage }
                        }
                    },
                    SetFields = new SetFields()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setFieldsErrorMessage1 },
                            new APIServerException() { Message = setFieldsErrorMessage2 },
                        }
                    },
                    SetTags = new SetTags()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTagsErrorMessage }
                        }
                    },
                    SetLinks = new SetLinks()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setLinksErrorMessage }
                        }
                    }
                }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiExceptionExtensions.Create(statusCode, headers, createEntryResult, innerException);

            var errorMessages = new string[] { setTemplateErrorMessage, setFieldsErrorMessage1, setFieldsErrorMessage2, setTagsErrorMessage, setLinksErrorMessage };
            string expectedMessage = $"{nameof(createEntryResult.Operations.EntryCreate.EntryId)}={entryId}. " + string.Join(" ", errorMessages);
            Assert.Equal(expectedMessage, exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(1, exception.ProblemDetails.Extensions.Count);
            Assert.Equal(createEntryResult, exception.ProblemDetails.Extensions[typeof(CreateEntryResult).Name]);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Null(exception.ProblemDetails.Type);
            Assert.Null(exception.ProblemDetails.Detail);
            Assert.Null(exception.ProblemDetails.Instance);
            Assert.Equal(default, exception.ProblemDetails.ErrorCode);
            Assert.Null(exception.ProblemDetails.ErrorSource);
            Assert.Null(exception.ProblemDetails.TraceId);
        }

        [Fact]
        public void Create_WithNullCreateEntryResult_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            CreateEntryResult createEntryResult = null;
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiExceptionExtensions.Create(statusCode, headers, createEntryResult, innerException);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.Extensions.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Null(exception.ProblemDetails.Type);
            Assert.Null(exception.ProblemDetails.Detail);
            Assert.Null(exception.ProblemDetails.Instance);
            Assert.Equal(default, exception.ProblemDetails.ErrorCode);
            Assert.Null(exception.ProblemDetails.ErrorSource);
            Assert.Null(exception.ProblemDetails.TraceId);
        }
    }
}
