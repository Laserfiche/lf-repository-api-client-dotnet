using Laserfiche.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class ApiExceptionExtensionsTest
    {
        private const string OPERATION_ID_HEADER = "X-RequestId";

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
                [OPERATION_ID_HEADER] = new List<string>() { operationId }
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
                [OPERATION_ID_HEADER] = new List<string>() { operationId }
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
                [OPERATION_ID_HEADER] = new List<string>() { operationId }
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
    }
}
