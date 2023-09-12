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
            _httpClient?.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            AttributesClient = new AttributesClient(_httpClient);
            AuditReasonsClient = new AuditReasonsClient(_httpClient);
            EntriesClient = new EntriesClient(_httpClient);
            FieldDefinitionsClient = new FieldDefinitionsClient(_httpClient);
            LinkDefinitionsClient = new LinkDefinitionsClient(_httpClient);
            RepositoriesClient = new RepositoriesClient(_httpClient);
            SearchesClient = new SearchesClient(_httpClient);
            SimpleSearchesClient = new SimpleSearchesClient(_httpClient);
            TagDefinitionsClient = new TagDefinitionsClient(_httpClient);
            TasksClient = new TasksClient(_httpClient);
            TemplateDefinitionsClient = new TemplateDefinitionsClient(_httpClient);
        }

        /// <summary>
        /// Create a Laserfiche repository client.
        /// </summary>
        /// <param name="httpRequestHandler">The http request handler for the Laserfiche repository client.</param>
        /// <param name="baseUrlDebug">(optional) Override for the Laserfiche repository API base url.</param>
        /// <returns>IRepositoryApiClient</returns>
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
        /// Create a Laserfiche repository client that will use Laserfiche Cloud OAuth client credentials to get access tokens.
        /// </summary>
        /// <param name="servicePrincipalKey">The service principal key created for the service principal from the Laserfiche Account Administration.</param>
        /// <param name="accessKey">The access key exported from the Laserfiche Developer Console.</param>
        /// <param name="scope">(optional) The requested space-delimited scopes for the access token.</param>
        /// <param name="baseUrlDebug">(optional) Override for the Laserfiche repository API base url.</param>
        /// <returns>IRepositoryApiClient</returns>
        public static IRepositoryApiClient CreateFromAccessKey(string servicePrincipalKey, AccessKey accessKey, string scope = null, string baseUrlDebug = null)
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(servicePrincipalKey, accessKey, scope);
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrlDebug);
        }

        /// <summary>
        /// Create a Laserfiche repository client that will use username and password to get access tokens for Laserfiche API. Password credentials grant type is implemented by the Laserfiche Self-Hosted API server. Not available in cloud.
        /// </summary>
        /// <param name="repositoryId">The repository ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="baseUrl">API server base URL e.g., https://{APIServerName}/LFRepositoryAPI.</param>
        /// <returns>IRepositoryApiClient</returns>
        public static IRepositoryApiClient CreateFromUsernamePassword(string repositoryId, string username, string password, string baseUrl)
        {
            var httpRequestHandler = new UsernamePasswordHandler(repositoryId, username, password, baseUrl);
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrl);
        }
    }
}
