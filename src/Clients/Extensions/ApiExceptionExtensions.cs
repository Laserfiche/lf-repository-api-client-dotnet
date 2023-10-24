// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

            ProblemDetails problemDetails = ProblemDetails.Create(statusCode, headers);
            problemDetails.Title = createEntryResult.GetSummary();
            problemDetails.Extensions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [createEntryResult.GetType().Name] = createEntryResult
            };
            return ApiException.Create(statusCode, headers, problemDetails, innerException);
        }
    }
}
