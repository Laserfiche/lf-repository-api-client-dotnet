using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public class ClientOptions : IClientOptions
    {
        public string Domain { get; set; }

        public Func<HttpRequestMessage, ILaserficheRepositoryApiClient, CancellationToken, Task> BeforeSendAsync { get; set; }
            = (request, repositoryClient, cancellationToken) => Task.CompletedTask;

        public Func<HttpResponseMessage, ILaserficheRepositoryApiClient, CancellationToken, Task<bool>> AfterSendAsync { get; set; }
            = (response, repositoryClient, cancellationToken) => Task.FromResult(false);
    }
}
