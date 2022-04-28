using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public interface IClientOptions
    {
        /// <summary>
        /// Invoked before a request with the request message, the repository client and cancellation token.
        /// Returns the access token.
        /// </summary>
        Func<HttpRequestMessage, ILaserficheRepositoryApiClient, CancellationToken, Task<string>> BeforeSendAsync { get; set; }

        /// <summary>
        /// Invoked after a request with the response message, the repository client and cancellation token.
        /// Returns true if the request should be retried.
        /// </summary>
        Func<HttpResponseMessage, ILaserficheRepositoryApiClient, CancellationToken, Task<bool>> AfterSendAsync { get; set; }
    }
}
