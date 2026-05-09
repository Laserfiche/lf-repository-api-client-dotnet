// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Util
{
    /// <summary>
    /// Process-wide cache of the OpenAPI <c>operationId</c> set served at
    /// <c>{BaseUrl}/swagger/v2/swagger.json</c>. Keyed by BaseUrl so a single test process
    /// targeting multiple environments caches each separately.
    ///
    /// Fetch happens once per (process, BaseUrl); subsequent <see cref="Get(string)"/> calls
    /// return the cached entry. Fetch failure is itself cached as <c>FetchError</c> so the
    /// (potentially slow) network call doesn't repeat once the environment is known to be
    /// unreachable.
    /// </summary>
    internal static class SwaggerOperationCache
    {
        private static readonly ConcurrentDictionary<string, (HashSet<string> Operations, Exception FetchError)> _cache
            = new ConcurrentDictionary<string, (HashSet<string>, Exception)>(StringComparer.Ordinal);

        public static (HashSet<string> Operations, Exception FetchError) Get(string baseUrl)
            => _cache.GetOrAdd(baseUrl ?? string.Empty, FetchOnce);

        private static (HashSet<string>, Exception) FetchOnce(string baseUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new ArgumentException("BaseUrl is empty; cannot probe swagger.");
                }

                var url = baseUrl.TrimEnd('/') + "/swagger/v2/swagger.json";
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var json = http.GetStringAsync(url).GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(json);
                var ops = new HashSet<string>(StringComparer.Ordinal);
                if (doc.RootElement.TryGetProperty("paths", out var paths) && paths.ValueKind == JsonValueKind.Object)
                {
                    foreach (var path in paths.EnumerateObject())
                    {
                        if (path.Value.ValueKind != JsonValueKind.Object) continue;
                        foreach (var method in path.Value.EnumerateObject())
                        {
                            if (method.Value.ValueKind == JsonValueKind.Object
                                && method.Value.TryGetProperty("operationId", out var idElem)
                                && idElem.ValueKind == JsonValueKind.String)
                            {
                                var id = idElem.GetString();
                                if (!string.IsNullOrEmpty(id)) ops.Add(id);
                            }
                        }
                    }
                }
                return (ops, null);
            }
            catch (Exception ex)
            {
                return (new HashSet<string>(StringComparer.Ordinal), ex);
            }
        }
    }
}
