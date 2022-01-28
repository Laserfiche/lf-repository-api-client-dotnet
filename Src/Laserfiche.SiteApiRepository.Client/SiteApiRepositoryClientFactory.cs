using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Laserfiche.SiteApiRepository.Client
{
    public static class SiteApiRepositoryClientFactory
    {
        public static V1.ISiteApiRepositoryClient CreateClientV1(ClientOptions options)
        {
            options = options ?? new ClientOptions();
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = options.AllowAutoRedirect
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{options.BaseUrl.Trim('/')}/")
            };
            return new V1.SiteApiRepositoryClient(httpClient);
        }

        public static V2.ISiteApiRepositoryClient CreateClientV2(ClientOptions options)
        {
            options = options ?? new ClientOptions();
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = options.AllowAutoRedirect
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{options.BaseUrl.Trim('/')}/")
            };
            return new V2.SiteApiRepositoryClient(httpClient);
        }
    }
}
