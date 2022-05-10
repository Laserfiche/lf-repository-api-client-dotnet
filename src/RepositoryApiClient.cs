using Laserfiche.Repository.Api.Client.Util;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    public interface IRepositoryApiClient
    {
        string AccessToken { get; set; }
        string RefreshToken { get; set; }

        /// <summary>
        /// The headers which should be sent with each request.
        /// </summary>
        System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders { get; }

        IAttributesClient AttributesClient { get; }
        IAuditReasonsClient AuditReasonsClient { get; }
        IEntriesClient EntriesClient { get; }
        IFieldDefinitionsClient FieldDefinitionsClient { get; }
        IRepositoriesClient RepositoriesClient { get; }
        ISearchesClient SearchesClient { get; }
        IServerSessionClient ServerSessionClient { get; }
        ISimpleSearchesClient SimpleSearchesClient { get; }
        ITagDefinitionsClient TagDefinitionsClient { get; }
        ITasksClient TasksClient { get; }
        ITemplateDefinitionsClient TemplateDefinitionsClient { get; }
    }

    public class RepositoryApiClient : IRepositoryApiClient
    {
        private readonly HttpClient _httpClient;

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _httpClient.DefaultRequestHeaders; }
        }

        public IAttributesClient AttributesClient { get; }
        public IAuditReasonsClient AuditReasonsClient { get; }
        public IEntriesClient EntriesClient { get; }
        public IFieldDefinitionsClient FieldDefinitionsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public ISearchesClient SearchesClient { get; }
        public IServerSessionClient ServerSessionClient { get; }
        public ISimpleSearchesClient SimpleSearchesClient { get; }
        public ITagDefinitionsClient TagDefinitionsClient { get; }
        public ITasksClient TasksClient { get; }
        public ITemplateDefinitionsClient TemplateDefinitionsClient { get; }

        public RepositoryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            AttributesClient = new AttributesClient(_httpClient);
            AuditReasonsClient = new AuditReasonsClient(_httpClient);
            EntriesClient = new EntriesClient(_httpClient);
            FieldDefinitionsClient = new FieldDefinitionsClient(_httpClient);
            RepositoriesClient = new RepositoriesClient(_httpClient);
            SearchesClient = new SearchesClient(_httpClient);
            ServerSessionClient = new ServerSessionClient(_httpClient);
            SimpleSearchesClient = new SimpleSearchesClient(_httpClient);
            TagDefinitionsClient = new TagDefinitionsClient(_httpClient);
            TasksClient = new TasksClient(_httpClient);
            TemplateDefinitionsClient = new TemplateDefinitionsClient(_httpClient);
        }

        /// <summary>
        /// Create a Laserfiche repository client.
        /// </summary>
        /// <param name="options">The client options for the Laserfiche repository client.</param>
        /// <param name="serviceBaseUrlDebug">Optional override for the Laserfiche repository API base url.</param>
        /// <param name="httpClientHandler">Optional HttpClientHandler.</param>
        /// <returns></returns>
        public static IRepositoryApiClient Create(IClientOptions options, string serviceBaseUrlDebug = "", HttpClientHandler httpClientHandler = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.BeforeSendAsync == null)
                throw new ArgumentNullException(nameof(options.BeforeSendAsync));
            if (options.AfterSendAsync == null)
                throw new ArgumentNullException(nameof(options.AfterSendAsync));

            httpClientHandler = httpClientHandler ?? new HttpClientHandler();
            var repositoryClientHandler = new RepositoryApiClientHandler(httpClientHandler, options, serviceBaseUrlDebug);
            var httpClient = new HttpClient(repositoryClientHandler)
            {
                BaseAddress = string.IsNullOrEmpty(serviceBaseUrlDebug) ? new Uri(RepositoryApiClientUtil.GetRepositoryBaseUri("")) : new Uri(serviceBaseUrlDebug)
            };
            var repositoryClient = new RepositoryApiClient(httpClient);
            repositoryClientHandler.Client = repositoryClient;
            return repositoryClient;
        }

    }
}
