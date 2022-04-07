using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public interface IClientOptions
    {
        /// <summary>
        /// The Laserfiche domain.
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// Invoked before a request with the request message, the repository client and cancellation token.
        /// </summary>
        Func<HttpRequestMessage, ILaserficheRepositoryApiClient, CancellationToken, Task> BeforeSendAsync { get; set; }

        /// <summary>
        /// Invoked after a request with the response message, the repository client and cancellation token.
        /// Return true if the request should be retried.
        /// </summary>
        Func<HttpResponseMessage, ILaserficheRepositoryApiClient, CancellationToken, Task<bool>> AfterSendAsync { get; set; }
    }
}
