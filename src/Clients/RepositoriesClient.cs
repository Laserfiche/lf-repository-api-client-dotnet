﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Laserfiche.Repository.Api.Client
{
    public partial class RepositoriesClient
    {
        /// <summary>
        /// Returns the repository resource list that current user has access to given the API server base URL. Only available in Laserfiche Self-Hosted.
        /// </summary>
        /// <param name="baseUrl">API server base URL e.g., https://{APIServerName}/LFRepositoryAPI.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get the respository resource list successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public static async System.Threading.Tasks.Task<ICollection<RepositoryInfo>> GetSelfHostedRepositoryListAsync(string baseUrl, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            using (HttpClient client_ = new HttpClient())
            {
                if (!string.IsNullOrEmpty(baseUrl))
                    baseUrl = baseUrl.TrimEnd('/') + "/";
                client_.BaseAddress = new Uri(baseUrl);
                RepositoriesClient repositoriesClient = new RepositoriesClient(client_);
                return await repositoriesClient.GetRepositoryListAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
