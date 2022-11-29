using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class ApiExceptionTest
    {
        private void AssertNullProblemDetailsOptionalProperties(ProblemDetails problemDetails)
        {
            Assert.Null(problemDetails.Type);
            Assert.Null(problemDetails.Detail);
            Assert.Null(problemDetails.Instance);
            Assert.Equal(default, problemDetails.ErrorCode);
            Assert.Null(problemDetails.ErrorSource);
            Assert.Null(problemDetails.TraceId);
        }

        [Fact]
        public void Create_WithNullHeaders_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;

            ApiException exception = ApiException.Create(statusCode, null, null);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Null(exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Null(exception.Headers);
            Assert.Null(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [Fact]
        public void Create_WithoutOperationIdHeader_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                ["Content-Type"] = new List<string>() { "application/json" }
            };

            ApiException exception = ApiException.Create(statusCode, headers, null);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Null(exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Null(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [Fact]
        public void Create_WithOperationIdHeader_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            
            ApiException exception = ApiException.Create(statusCode, headers, null);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Null(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [Fact]
        public void Create_WithInnerException_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, innerException);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
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
                AdditionalProperties = new Dictionary<string, object>() { ["key"] = "value" }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetails, innerException);

            Assert.Equal(problemDetails.Title, exception.ProblemDetails.Title);
            Assert.Equal(problemDetails.Type, exception.ProblemDetails.Type);
            Assert.Equal(problemDetails.Detail, exception.ProblemDetails.Detail);
            Assert.Equal(problemDetails.Status, exception.ProblemDetails.Status);
            Assert.Equal(problemDetails.Instance, exception.ProblemDetails.Instance);
            Assert.Equal(problemDetails.OperationId, exception.ProblemDetails.OperationId);
            Assert.Equal(problemDetails.ErrorCode, exception.ProblemDetails.ErrorCode);
            Assert.Equal(problemDetails.ErrorSource, exception.ProblemDetails.ErrorSource);
            Assert.Equal(problemDetails.TraceId, exception.ProblemDetails.TraceId);
            Assert.Equal(problemDetails.AdditionalProperties, exception.ProblemDetails.AdditionalProperties);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void Create_WithNullProblemDetails_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = null;
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetails, innerException);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
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

            ApiException exception = ApiException.Create(statusCode, headers, createEntryResult, innerException);

            var errorMessages = new string[] { setTemplateErrorMessage, setFieldsErrorMessage1, setFieldsErrorMessage2, setTagsErrorMessage, setLinksErrorMessage };
            string expectedMessage = $"{nameof(createEntryResult.Operations.EntryCreate.EntryId)}={entryId}. " + string.Join(" ", errorMessages);
            Assert.Equal(expectedMessage, exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(1, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(createEntryResult, exception.ProblemDetails.AdditionalProperties[typeof(CreateEntryResult).Name]);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
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

            ApiException exception = ApiException.Create(statusCode, headers, createEntryResult, innerException);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
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
                AdditionalProperties = new Dictionary<string, object>() { ["key"] = "value" }
            };
            string problemDetailsResponseString = JsonConvert.SerializeObject(problemDetails);
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetailsResponseString, null, innerException);

            Assert.Equal(problemDetails.Title, exception.ProblemDetails.Title);
            Assert.Equal(problemDetails.Type, exception.ProblemDetails.Type);
            Assert.Equal(problemDetails.Detail, exception.ProblemDetails.Detail);
            Assert.Equal(problemDetails.Status, exception.ProblemDetails.Status);
            Assert.Equal(problemDetails.Instance, exception.ProblemDetails.Instance);
            Assert.Equal(problemDetails.OperationId, exception.ProblemDetails.OperationId);
            Assert.Equal(problemDetails.ErrorCode, exception.ProblemDetails.ErrorCode);
            Assert.Equal(problemDetails.ErrorSource, exception.ProblemDetails.ErrorSource);
            Assert.Equal(problemDetails.TraceId, exception.ProblemDetails.TraceId);
            Assert.Equal(problemDetails.AdditionalProperties, exception.ProblemDetails.AdditionalProperties);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void Create_WithResponseString_ResponseStringIsNotProblemDetails_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ApiException.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            var responseString = JsonConvert.SerializeObject(new Folder() { Id = 123, Name = "Folder" });
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, responseString, null, innerException);

            Assert.Equal($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.Equal(statusCode, exception.ProblemDetails.Status);
            Assert.Equal(operationId, exception.ProblemDetails.OperationId);
            Assert.Equal(0, exception.ProblemDetails.AdditionalProperties.Count);
            Assert.Equal(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.Equal(exception.ProblemDetails.Title, exception.Message);
            Assert.Equal(headers, exception.Headers);
            Assert.Equal(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }
    }
}
