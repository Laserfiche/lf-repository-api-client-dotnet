// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.HttpHandlers;
using Laserfiche.Api.Client.OAuth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Table.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository API client.
    /// </summary>
    public class TableApiClient : ITableApiClient
    {
        private const string DefaultBaseAddress = "https://api.a.clouddev.laserfiche.ca/table/";
        private readonly HttpClient _httpClient;

        /// <inheritdoc/>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _httpClient.DefaultRequestHeaders; }
        }

        /// <inheritdoc/>
        public ITableClient TableClient { get; }

        internal TableApiClient(HttpClient httpClient)
        {
            string OAuthAccessToken = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkYW5mZW5nLmNoZW5AbGFzZXJmaWNoZS5jb20iLCJjbGllbnRfaWQiOiIyYjM4MmRmZC1hNzI1LTRlN2YtYWM2Mi0xNTI4ZTllMDRhZDciLCJjc2lkIjoiMTEyMzQ1Njc4OSIsInRyaWQiOiIxMDY0IiwibmFtZSI6ImRhbmZlbmcuY2hlbiIsInV0eXAiOiJGdWxsIiwiZ3RpZCI6IjEyMjQ5IiwiYWNzc2siOiJJcVZXTXh5TTBYTkowRElQQitLN2lENzZ4QWJ1RWJWTWZyb2lZbVhRN2JiWHNwUUswSkNNc3U5aG11UE0rOFluIiwidGNpZCI6IjllYzkzODZhLTQyY2UtNDljYy1hNDhhLWNiZjQ5ZWRhMDU1OCIsImF1ZCI6ImNhLmNlbnRyYWwuY29tIiwiZXhwIjoxNjk5NTAzMjk3LCJpc3MiOiJzaWduaW4ubGFzZXJmaWNoZS5jb20iLCJuYmYiOjE2OTk0OTkwMjUsImlhdCI6MTY5OTQ5OTY5N30.rYWEXDvk4T4__25B_EEWX3X9h5cpurszVpIzrK9r8x4HR-itxABmYIGGRwYb5YRi7zSe0n4jj5o-c6UNtfxFMw";
            _httpClient = httpClient;
            _httpClient?.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            _httpClient?.DefaultRequestHeaders.Add("Authorization", $"Bearer {OAuthAccessToken}");
            TableClient = new TableClient(_httpClient);
        }

        /// <summary>
        /// Create a Laserfiche repository client.
        /// </summary>
        /// <param name="httpRequestHandler">The http request handler for the Laserfiche repository client.</param>
        /// <param name="baseUrlDebug">(optional) Override for the Laserfiche repository API base url.</param>
        /// <returns>IRepositoryApiClient</returns>
        public static ITableApiClient CreateFromHttpRequestHandler(IHttpRequestHandler httpRequestHandler, string baseUrlDebug = null)
        {
            if (httpRequestHandler == null)
                throw new ArgumentNullException(nameof(httpRequestHandler));

            if (!string.IsNullOrEmpty(baseUrlDebug))
                baseUrlDebug = baseUrlDebug.TrimEnd('/') + "/";

            var repositoryClientHandler = new TableApiClientHandler(httpRequestHandler, baseUrlDebug);
            var httpClient = new HttpClient(repositoryClientHandler);

            if (httpRequestHandler is UsernamePasswordHandler)
            {
                httpClient.BaseAddress = new Uri(baseUrlDebug);
            }
            else
            {
                httpClient.BaseAddress = new Uri(DefaultBaseAddress);
            }

            var repositoryClient = new TableApiClient(httpClient);
            return repositoryClient;
        }

        /// <summary>
        /// Create a Laserfiche repository client that will use Laserfiche Cloud OAuth client credentials to get access tokens.
        /// </summary>
        /// <param name="servicePrincipalKey">The service principal key created for the service principal from the Laserfiche Account Administration.</param>
        /// <param name="accessKey">The access key exported from the Laserfiche Developer Console.</param>
        /// <param name="scope">(optional) The requested space-delimited scopes for the access token.</param>
        /// <param name="baseUrlDebug">(optional) Override for the Laserfiche repository API base url.</param>
        /// <returns>IRepositoryApiClient</returns>
        public static ITableApiClient CreateFromAccessKey(string servicePrincipalKey, AccessKey accessKey, string scope = null, string baseUrlDebug = null)
        {
            //var httpRequestHandler = new OAuthClientCredentialsHandler(servicePrincipalKey, accessKey, scope);
            var httpRequestHandler = new TableHttpRequestHandler();
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrlDebug);
        }
    }

    public class TableHttpRequestHandler : IHttpRequestHandler
    {
        public Task<bool> AfterSendAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            return Task.FromResult(new BeforeSendResult());
        }
    }
}
