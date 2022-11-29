using Laserfiche.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laserfiche.Repository.Api.Client
{
    internal static class ApiExceptionExtensions
    {
        internal static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, string response, JsonSerializerSettings jsonSerializerSettings, Exception innerException)
        {
            return ApiException.Create(statusCode, headers, response, jsonSerializerSettings, innerException);
        }

        internal static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, ProblemDetails problemDetails, Exception innerException)
        {
            return ApiException.Create(statusCode, headers, problemDetails, innerException);
        }

        internal static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
        {
            return ApiException.Create(statusCode, headers, innerException);
        }

        internal static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, CreateEntryResult createEntryResult, Exception innerException)
        {
            if (createEntryResult == null)
            {
                return ApiException.Create(statusCode, headers, innerException);
            }

            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = createEntryResult.GetSummary(),
                Status = statusCode,
                OperationId = headers?.TryGetValue(ApiException.OPERATION_ID_HEADER, out IEnumerable<string> operationIdHeader) == true ? operationIdHeader.FirstOrDefault() : null,
                Extensions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { [createEntryResult.GetType().Name] = createEntryResult }
            };
            return ApiException.Create(statusCode, headers, problemDetails, innerException);
        }
    }
}
