using Laserfiche.Api.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Repositories API client.
    /// </summary>
    partial interface IRepositoriesClient { }

    /// <summary>
    /// The Laserfiche Repository Repositories API client.
    /// </summary>
    public partial class RepositoriesClient
    {
        /// <summary>
        /// Returns the repository resource list that current user has access to given the API server base URL. Only available in Laserfiche Self-Hosted.
        /// </summary>
        /// <param name="baseUrl">API server base URL e.g., https://{APIServerName}/LFRepositoryAPI.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of respositories.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public static async Task<RepositoryCollectionResponse> ListSelfHostedRepositoriesAsync(string baseUrl, ListRepositoriesParameters parameters = null, CancellationToken cancellationToken = default)
        {
            using (HttpClient client_ = new HttpClient())
            {
                if (!string.IsNullOrEmpty(baseUrl))
                    baseUrl = baseUrl.TrimEnd('/') + "/";
                client_.BaseAddress = new Uri(baseUrl);
                RepositoriesClient repositoriesClient = new RepositoriesClient(client_);
                return await repositoriesClient.ListRepositoriesAsync(parameters, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
