using Laserfiche.Repository.Api.Client.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// A factory for creating client options for the Laserfiche repository client.
    /// </summary>
    public static class ClientOptionsFactory
    {
        private static readonly ISet<string> ServerSessionCreated = new HashSet<string>();

        /// <summary>
        /// Create client options with automatic access token refresh.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <param name="getAccessToken">Delegate function that will be invoked when getting an access token.</param>
        /// <param name="refreshAccessToken">Delegate function that will be invoked when refreshing an access token.</param>
        /// <returns>A ClientOptions for the Laserfiche repository client.</returns>
        public static ClientOptions CreateClientOptions(string domain,
            Func<CancellationToken, Task<(string accessToken, string refreshToken)>> getAccessToken,
            Func<string, CancellationToken, Task<(string accessToken, string refreshToken)>> refreshAccessToken)
        {
            return new ClientOptions()
            {
                Domain = domain,
                BeforeSendAsync = (request, repositoryClient, cancellationToken) => BeforeSendAsync(request, repositoryClient, getAccessToken, cancellationToken),
                AfterSendAsync = (response, repositoryClient, cancellationToken) => AfterSendAsync(response, repositoryClient, getAccessToken, refreshAccessToken, cancellationToken)
            };
        }

        /// <summary>
        /// A BeforeSendAsync implementation that will automatically get an access token when one does not exist in the
        /// repository client.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="repositoryClient"></param>
        /// <param name="getAccessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task BeforeSendAsync(HttpRequestMessage request, ILaserficheRepositoryApiClient repositoryClient,
            Func<CancellationToken, Task<(string accessToken, string refreshToken)>> getAccessToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(repositoryClient.AccessToken))
            {
                repositoryClient.RefreshToken = null;
                (string accessToken, string refreshToken) = await getAccessToken(cancellationToken);
                repositoryClient.AccessToken = accessToken;
                repositoryClient.RefreshToken = refreshToken;
            }

            string repoId = LaserficheRepositoryApiClientUtil.ExtractRepositoryIdFromUri(request.RequestUri);
            if (!string.IsNullOrEmpty(repoId) && !ServerSessionCreated.Contains(repoId) && !string.IsNullOrEmpty(repositoryClient.AccessToken)
                && !request.RequestUri.AbsolutePath.EndsWith("/ServerSession/Create", StringComparison.OrdinalIgnoreCase))
            {
                await repositoryClient.CreateServerSessionAsync(repoId, cancellationToken);
            }
        }

        /// <summary>
        /// An AfterSendAsync implementation that will automatically refresh an access token when the current access token
        /// expires.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="repositoryClient"></param>
        /// <param name="getAccessToken"></param>
        /// <param name="refreshAccessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> AfterSendAsync(HttpResponseMessage response, ILaserficheRepositoryApiClient repositoryClient,
            Func<CancellationToken, Task<(string accessToken, string refreshToken)>> getAccessToken,
            Func<string, CancellationToken, Task<(string accessToken, string refreshToken)>> refreshAccessToken,
            CancellationToken cancellationToken)
        {
            string repoId = LaserficheRepositoryApiClientUtil.ExtractRepositoryIdFromUri(response.RequestMessage.RequestUri);
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(repoId))
            {
                if (!ServerSessionCreated.Contains(repoId) && response.RequestMessage.RequestUri.AbsolutePath.EndsWith("/ServerSession/Create", StringComparison.OrdinalIgnoreCase))
                {
                    ServerSessionCreated.Add(repoId);
                }
                else if (response.RequestMessage.RequestUri.AbsolutePath.EndsWith("/ServerSession/Invalidate", StringComparison.OrdinalIgnoreCase))
                {
                    ServerSessionCreated.Remove(repoId);
                }
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ServerSessionCreated.Remove(repoId);
                if (string.IsNullOrEmpty(repositoryClient.AccessToken))
                {
                    repositoryClient.RefreshToken = null;
                    (repositoryClient.AccessToken, repositoryClient.RefreshToken) = await getAccessToken(cancellationToken);
                }
                else
                {
                    repositoryClient.AccessToken = null;
                    if (string.IsNullOrEmpty(repositoryClient.RefreshToken))
                    {
                        try
                        {
                            (repositoryClient.AccessToken, repositoryClient.RefreshToken) = await refreshAccessToken(repositoryClient.RefreshToken, cancellationToken);
                        }
                        catch
                        {
                            repositoryClient.RefreshToken = null;
                            (repositoryClient.AccessToken, repositoryClient.RefreshToken) = await getAccessToken(cancellationToken);
                        }
                    }
                    else
                    {
                        (repositoryClient.AccessToken, repositoryClient.RefreshToken) = await getAccessToken(cancellationToken);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
