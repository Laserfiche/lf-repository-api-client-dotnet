using System;
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
        /// <summary>
        /// Create client options with automatic access token refresh.
        /// </summary>
        /// <param name="getAccessToken">Delegate function that will be invoked when getting an access token.</param>
        /// <param name="refreshAccessToken">Delegate function that will be invoked when refreshing an access token.</param>
        /// <returns>A ClientOptions for the Laserfiche repository client.</returns>
        public static ClientOptions CreateClientOptions(Func<CancellationToken, Task<(string accessToken, string refreshToken)>> getAccessToken,
            Func<string, CancellationToken, Task<(string accessToken, string refreshToken)>> refreshAccessToken)
        {
            return new ClientOptions()
            {
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
        public static async Task<string> BeforeSendAsync(HttpRequestMessage request, IRepositoryApiClient repositoryClient,
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

            return repositoryClient.AccessToken;
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
        public static async Task<bool> AfterSendAsync(HttpResponseMessage response, IRepositoryApiClient repositoryClient,
            Func<CancellationToken, Task<(string accessToken, string refreshToken)>> getAccessToken,
            Func<string, CancellationToken, Task<(string accessToken, string refreshToken)>> refreshAccessToken,
            CancellationToken cancellationToken)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
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
