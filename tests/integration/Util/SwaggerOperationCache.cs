// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Util
{
    /// <summary>
    /// Process-wide cache of the OpenAPI <c>operationId</c> set served at
    /// <c>{BaseUrl}/swagger/v2/swagger.json</c>. Keyed by BaseUrl so a single test process
    /// targeting multiple environments caches each separately.
    ///
    /// Fetch happens once per (process, BaseUrl); subsequent <see cref="GetAsync(string)"/>
    /// calls return the cached <see cref="Task{TResult}"/>. Fetch failure is itself cached as
    /// <c>FetchError</c> so the (potentially slow) network call doesn't repeat once the
    /// environment is known to be unreachable.
    /// </summary>
    internal static class SwaggerOperationCache
    {
        // Cache the Task itself so all callers await the same in-flight fetch.
        // ConcurrentDictionary.GetOrAdd can invoke the factory more than once under contention,
        // but only one Task wins as the cached value; the loser completes and is discarded.
        // For test infra that costs at most one extra HTTP request at process start — benign.
        private static readonly ConcurrentDictionary<string, Task<(HashSet<string> Operations, Exception FetchError)>> _cache
            = new ConcurrentDictionary<string, Task<(HashSet<string>, Exception)>>(StringComparer.Ordinal);

        public static Task<(HashSet<string> Operations, Exception FetchError)> GetAsync(string baseUrl)
            => _cache.GetOrAdd(baseUrl ?? string.Empty, FetchOnceAsync);

        private static async Task<(HashSet<string>, Exception)> FetchOnceAsync(string baseUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new ArgumentException("BaseUrl is empty; cannot probe swagger.");
                }

                var url = baseUrl.TrimEnd('/') + "/swagger/v2/swagger.json";
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var json = await http.GetStringAsync(url);

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
