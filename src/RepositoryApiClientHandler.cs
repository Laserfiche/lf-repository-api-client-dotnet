using Laserfiche.Repository.Api.Client.Util;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public class RepositoryApiClientHandler : DelegatingHandler
    {
        public IRepositoryApiClient Client { get; set; }
        public IClientOptions ClientOptions { get; set; }
        private JsonWebToken _AccessToken { get; set; }
        private string _BaseUriHost { get; set; }
        private bool _UseServiceBaseUrlDebug { get; set; }

        public RepositoryApiClientHandler(HttpMessageHandler httpMessageHandler, IClientOptions options, string serviceBaseUrlDebug = "") : base(httpMessageHandler)
        {
            ClientOptions = options ?? throw new ArgumentNullException(nameof(options));
            _UseServiceBaseUrlDebug = !string.IsNullOrEmpty(serviceBaseUrlDebug);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await SendWithRetryAsync(request, cancellationToken, true)
                ?? await SendWithRetryAsync(request, cancellationToken, false);
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool willRetry)
        {
            HttpResponseMessage response;
            string accessToken = await ClientOptions.BeforeSendAsync(request, Client, cancellationToken);

            if (!_UseServiceBaseUrlDebug)
            {
                if (_AccessToken == null || accessToken != _AccessToken.EncodedToken)
                {
                    _AccessToken = JwtUtil.ReadJWT(accessToken);
                    string accountId = JwtUtil.GetAccountIdFromJwt(_AccessToken);
                    _BaseUriHost = RepositoryApiClientUtil.GetRepositoryBaseUriHost(accountId);
                }

                var requestUri = new UriBuilder(request.RequestUri)
                {
                    Host = _BaseUriHost
                };
                request.RequestUri = requestUri.Uri;
            }

            try
            {
                SetRequestAuthorizationHeader(request);
                response = await base.SendAsync(request, cancellationToken);
                if (willRetry && IsTransientHttpStatusCode(response.StatusCode) && IsIdempotentHttpMethod(request.Method))
                {
                    return null;
                }
            }
            catch
            {
                if (willRetry)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            bool shouldRetry = await ClientOptions.AfterSendAsync(response, Client, cancellationToken);
            if (willRetry && shouldRetry)
            {
                return null;
            }
            return response;
        }

        private void SetRequestAuthorizationHeader(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Client.AccessToken);
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
