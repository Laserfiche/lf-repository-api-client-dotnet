using Laserfiche.Api.Client.HttpHandlers;
using Laserfiche.Api.Client.OAuth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository API client.
    /// </summary>
    public class RepositoryApiClient : IRepositoryApiClient
    {
        private const string DefaultBaseAddress = "https://dummy.example.com/repository/";
        private readonly HttpClient _httpClient;

        /// <inheritdoc/>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _httpClient.DefaultRequestHeaders; }
        }

        /// <inheritdoc/>
        public IAttributesClient AttributesClient { get; }
        /// <inheritdoc/>
        public IAuditReasonsClient AuditReasonsClient { get; }
        /// <inheritdoc/>
        public IEntriesClient EntriesClient { get; }
        /// <inheritdoc/>
        public IFieldDefinitionsClient FieldDefinitionsClient { get; }
        /// <inheritdoc/>
        public ILinkDefinitionsClient LinkDefinitionsClient { get; }
        /// <inheritdoc/>
        public IRepositoriesClient RepositoriesClient { get; }
        /// <inheritdoc/>
        public ISearchesClient SearchesClient { get; }
        /// <inheritdoc/>
        public IServerSessionClient ServerSessionClient { get; }
        /// <inheritdoc/>
        public ISimpleSearchesClient SimpleSearchesClient { get; }
        /// <inheritdoc/>
        public ITagDefinitionsClient TagDefinitionsClient { get; }
        /// <inheritdoc/>
        public ITasksClient TasksClient { get; }
        /// <inheritdoc/>
        public ITemplateDefinitionsClient TemplateDefinitionsClient { get; }

        internal RepositoryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            AttributesClient = new AttributesClient(_httpClient);
            AuditReasonsClient = new AuditReasonsClient(_httpClient);
            EntriesClient = new EntriesClient(_httpClient);
            FieldDefinitionsClient = new FieldDefinitionsClient(_httpClient);
            LinkDefinitionsClient = new LinkDefinitionsClient(_httpClient);
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
        /// <param name="httpRequestHandler">The http request handler for the Laserfiche repository client.</param>
        /// <param name="baseUrlDebug">Optional override for the Laserfiche repository API base url.</param>
        /// <returns></returns>
        public static IRepositoryApiClient CreateFromHttpRequestHandler(IHttpRequestHandler httpRequestHandler, string baseUrlDebug = null)
        {
            if (httpRequestHandler == null)
                throw new ArgumentNullException(nameof(httpRequestHandler));

            if (!string.IsNullOrEmpty(baseUrlDebug))
                baseUrlDebug = baseUrlDebug.TrimEnd('/') + "/";

            var repositoryClientHandler = new RepositoryApiClientHandler(httpRequestHandler, baseUrlDebug);
            var httpClient = new HttpClient(repositoryClientHandler);

            if (httpRequestHandler is UsernamePasswordHandler)
            {
                httpClient.BaseAddress = new Uri(baseUrlDebug);
            }
            else
            {
                httpClient.BaseAddress = new Uri(DefaultBaseAddress);
            }

            var repositoryClient = new RepositoryApiClient(httpClient);
            return repositoryClient;
        }

        /// <summary>
        /// Create a Laserfiche repository client that will use OAuth client credentials to get access tokens.
        /// </summary>
        /// <param name="servicePrincipalKey"></param>
        /// <param name="accessKey"></param>
        /// <param name="baseUrlDebug"> Optional override for the Laserfiche repository API base url.</param>
        /// <returns></returns>
        public static IRepositoryApiClient CreateFromAccessKey(string servicePrincipalKey, AccessKey accessKey, string baseUrlDebug = null)
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(servicePrincipalKey, accessKey);
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrlDebug);
        }

        /// <summary>
        /// Create a Laserfiche repository client that will use username and password to get access tokens for Laserfiche API. Password credentials grant type is implemented by the self-hosted API server. Not available in cloud.
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="baseUrl">API server base URL e.g., https://example.com/LFRepositoryAPI/</param>
        /// <returns></returns>
        public static IRepositoryApiClient CreateFromUsernamePassword(string repoId, string username, string password, string baseUrl)
        {
            var httpRequestHandler = new UsernamePasswordHandler(repoId, username, password, baseUrl);
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrl);
        }
    }
}
