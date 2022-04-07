using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public class LaserficheRepositoryApiClientHandler : DelegatingHandler
    {
        public ILaserficheRepositoryApiClient Client { get; set; }
        public ClientOptions ClientOptions { get; set; }

        public LaserficheRepositoryApiClientHandler(HttpMessageHandler httpMessageHandler, ClientOptions options) : base(httpMessageHandler)
        {
            ClientOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await SendWithRetryAsync(request, cancellationToken, true)
                ?? await SendWithRetryAsync(request, cancellationToken, false);
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool willRetry)
        {
            HttpResponseMessage response;
            await ClientOptions.BeforeSendAsync(request, Client, cancellationToken);

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
