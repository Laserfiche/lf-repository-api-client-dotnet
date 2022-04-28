using Laserfiche.Repository.Api.Client.Util;
using System;
using System.Net.Http;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// A factory for creating a Laserfiche repository client.
    /// </summary>
    public static class LaserficheRepositoryApiClientFactory
    {
        /// <summary>
        /// Create a Laserfiche repository client.
        /// </summary>
        /// <param name="options">The client options for the Laserfiche repository client.</param>
        /// <param name="serviceBaseUrlDebug">Optional override for the Laserfiche repository API base url.</param>
        /// <param name="httpClientHandler">Optional HttpClientHandler.</param>
        /// <returns></returns>
        public static ILaserficheRepositoryApiClient CreateClient(ClientOptions options, string serviceBaseUrlDebug = "", HttpClientHandler httpClientHandler = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.BeforeSendAsync == null)
                throw new ArgumentNullException(nameof(options.BeforeSendAsync));
            if (options.AfterSendAsync == null)
                throw new ArgumentNullException(nameof(options.AfterSendAsync));

            httpClientHandler = httpClientHandler ?? new HttpClientHandler();
            var repositoryClientHandler = new LaserficheRepositoryApiClientHandler(httpClientHandler, options, serviceBaseUrlDebug);
            var httpClient = new HttpClient(repositoryClientHandler)
            {
                BaseAddress = string.IsNullOrEmpty(serviceBaseUrlDebug) ? new Uri(LaserficheRepositoryApiClientUtil.GetRepositoryBaseUri("")) : new Uri(serviceBaseUrlDebug)
            };
            var repositoryClient = new LaserficheRepositoryApiClient(httpClient);
            repositoryClientHandler.Client = repositoryClient;
            return repositoryClient;
        }
    }
}
