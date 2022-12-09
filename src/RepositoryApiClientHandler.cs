﻿using Laserfiche.Api.Client;
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

        private int _maxRetryDelay;
        private readonly Random _randomNumGenerator;

        public RepositoryApiClientHandler(IHttpRequestHandler httpRequestHandler, string baseUrlDebug) : base(new HttpClientHandler())
        {
            _httpRequestHandler = httpRequestHandler ?? throw new ArgumentNullException(nameof(httpRequestHandler));
            _baseUrlDebug = baseUrlDebug;
            _maxRetryDelay = 3;
            _randomNumGenerator= new Random();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await SendExAsync(request, cancellationToken, true)
                ?? await SendExAsync(request, cancellationToken, false);
        }

        private async Task<HttpResponseMessage> SendExAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool returnNullIfRetriable)
        {
            HttpResponseMessage response;

            // Sets the authorization header
            BeforeSendResult beforeSendResult;
            try
            {
                beforeSendResult = await _httpRequestHandler.BeforeSendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                if (returnNullIfRetriable)
                {
                    Console.WriteLine($"Exception caught at retrieving access token: {ex.Message}");
                    if (ex is ApiException)
                    {
                        Console.WriteLine($"Status code: {(ex as ApiException).StatusCode}");
                        Console.WriteLine($"ProblemDetails.Title: {(ex as ApiException).ProblemDetails.Title}");
                        Console.WriteLine($"ProblemDetails.Type: {(ex as ApiException).ProblemDetails.Type}");
                        Console.WriteLine($"ProblemDetails.Detail: {(ex as ApiException).ProblemDetails.Detail}");
                    }
                    return null;
                }
                else
                {
                    Console.WriteLine($"Exception thrown after re-try: {ex.Message}");
                    if (ex is ApiException)
                    {
                        Console.WriteLine($"Status code: {(ex as ApiException).StatusCode}");
                        Console.WriteLine($"ProblemDetails.Title: {(ex as ApiException).ProblemDetails.Title}");
                        Console.WriteLine($"ProblemDetails.Type: {(ex as ApiException).ProblemDetails.Type}");
                        Console.WriteLine($"ProblemDetails.Detail: {(ex as ApiException).ProblemDetails.Detail}");
                    }
                    throw;
                }
            }

            if (_baseAddress == null || (_baseUrlDebug == null && !_baseAddress.Host.EndsWith(beforeSendResult.RegionalDomain)))
            {
                _baseAddress = GetBaseAddress(beforeSendResult.RegionalDomain);
            }
            request.RequestUri = new Uri(_baseAddress, request.RequestUri.PathAndQuery);

            try
            {
                response = await base.SendAsync(request, cancellationToken);
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

            bool shouldRetry = await _httpRequestHandler.AfterSendAsync(response, cancellationToken);
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
