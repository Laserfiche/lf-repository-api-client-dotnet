using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Laserfiche.SiteApiRepository.Client.Test")]
namespace Laserfiche.SiteApiRepository.Client.V2
{
    partial interface ISiteApiRepositoryClient
    {
        void SetAuthToken(string accessToken);
        string GetAuthToken();
        void RemoveAuthToken();
    }

    partial class SiteApiRepositoryClient : ISiteApiRepositoryClient
    {
        public void SetAuthToken(string accessToken)
        {
            var authentication = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Authorization = authentication;
        }

        public string GetAuthToken()
        {
            string authToken = _httpClient.DefaultRequestHeaders?.Authorization?.Parameter;
            return authToken;
        }

        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
