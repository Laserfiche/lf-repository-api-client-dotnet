using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// Configuration parameters to create the API client.
    /// </summary>
    public class ClientOptions : IClientOptions
    {
        /// <inheritdoc/>
        public Func<HttpRequestMessage, IRepositoryApiClient, CancellationToken, Task<string>> BeforeSendAsync { get; set; }
            = (request, repositoryClient, cancellationToken) => Task.FromResult((string)null);

        /// <inheritdoc/>
        public Func<HttpResponseMessage, IRepositoryApiClient, CancellationToken, Task<bool>> AfterSendAsync { get; set; }
            = (response, repositoryClient, cancellationToken) => Task.FromResult(false);
    }
}
