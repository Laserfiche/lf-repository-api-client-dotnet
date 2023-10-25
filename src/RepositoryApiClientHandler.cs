// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.HttpHandlers;
using Laserfiche.Api.Client.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    internal class RepositoryApiClientHandler : DelegatingHandler
    {
        private readonly IHttpRequestHandler _httpRequestHandler;
        private readonly string _baseUrlDebug;
        private Uri _baseAddress;

        public RepositoryApiClientHandler(IHttpRequestHandler httpRequestHandler, string baseUrlDebug) : base(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
        {
            _httpRequestHandler = httpRequestHandler ?? throw new ArgumentNullException(nameof(httpRequestHandler));
            _baseUrlDebug = baseUrlDebug;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await SendExAsync(request, cancellationToken, true).ConfigureAwait(false)
                ?? await SendExAsync(request, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> SendExAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool returnNullIfRetriable)
        {
            HttpResponseMessage response;

            // Sets the authorization header
            var beforeSendResult = await _httpRequestHandler.BeforeSendAsync(request, cancellationToken).ConfigureAwait(false);

            if (_baseAddress == null || (_baseUrlDebug == null && !_baseAddress.Host.EndsWith(beforeSendResult.RegionalDomain)))
            {
                _baseAddress = GetBaseAddress(beforeSendResult.RegionalDomain);
            }
            request.RequestUri = new Uri(_baseAddress, request.RequestUri.PathAndQuery);

            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (returnNullIfRetriable && IsTransientHttpStatusCode(response.StatusCode) && IsIdempotentHttpMethod(request.Method))
                {
                    return null;
                }
            }
            catch
            {
                if (returnNullIfRetriable)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            bool shouldRetry = await _httpRequestHandler.AfterSendAsync(response, cancellationToken).ConfigureAwait(false);
            if (returnNullIfRetriable && shouldRetry)
            {
                return null;
            }
            return response;
        }

        private Uri GetBaseAddress(string domain)
        {
            string baseAddress = _baseUrlDebug ?? DomainUtils.GetRepositoryApiBaseUri(domain);
            return new Uri(baseAddress);
        }

        private static bool IsTransientHttpStatusCode(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 500 || statusCode == HttpStatusCode.RequestTimeout;
        }

        private static bool IsIdempotentHttpMethod(HttpMethod method)
        {
            return method == HttpMethod.Get || method == HttpMethod.Put || method == HttpMethod.Options;
        }
    }
}
