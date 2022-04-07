using Laserfiche.Repository.Api.Client.Util;
using System;
using System.Net.Http;

namespace Laserfiche.Repository.Api.Client
{
    public static class LaserficheRepositoryApiClientFactory
    {
        public static ILaserficheRepositoryApiClient CreateClientV1(ClientOptions options, HttpClientHandler httpClientHandler = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.Domain))
                throw new ArgumentNullException(nameof(options.Domain));
            if (options.BeforeSendAsync == null)
                throw new ArgumentNullException(nameof(options.BeforeSendAsync));
            if (options.AfterSendAsync == null)
                throw new ArgumentNullException(nameof(options.AfterSendAsync));

            string baseUri = LaserficheRepositoryApiClientUtil.GetRepositoryBaseUri(options.Domain);
            httpClientHandler = httpClientHandler ?? new HttpClientHandler();
            var repositoryClientHandler = new LaserficheRepositoryApiClientHandler(httpClientHandler, options);
            var httpClient = new HttpClient(repositoryClientHandler)
            {
                BaseAddress = new Uri(baseUri)
            };
            var repositoryClient = new LaserficheRepositoryApiClient(httpClient);
            repositoryClientHandler.Client = repositoryClient;
            return repositoryClient;
        }
    }
}
