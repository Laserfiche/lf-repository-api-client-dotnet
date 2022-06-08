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
        private const string _defaultBaseAddress = "https://dummy.example.com/repository/";
        private readonly HttpClient _httpClient;

        /// <inheritdoc/>
        public HttpRequestHeaders DefaultRequestHeaders
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

        internal RepositoryApiClient(HttpClient httpClient)
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
        /// <param name="httpRequestHandler">The http request handler for the Laserfiche repository client.</param>
        /// <param name="baseUrlDebug">Optional override for the Laserfiche repository API base url.</param>
        /// <returns></returns>
        public static IRepositoryApiClient CreateFromHttpRequestHandler(IHttpRequestHandler httpRequestHandler, string baseUrlDebug = null)
        {
            if (httpRequestHandler == null)
                throw new ArgumentNullException(nameof(httpRequestHandler));

            var repositoryClientHandler = new RepositoryApiClientHandler(httpRequestHandler, baseUrlDebug);
            var httpClient = new HttpClient(repositoryClientHandler)
            {
                BaseAddress = new Uri(_defaultBaseAddress)
            };
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
        /// Create a Laserfiche repository client that will use LFDS username and password to get access tokens.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="organization"></param>
        /// <param name="repoID"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IRepositoryApiClient CreateFromLfdsUsernamePassword(string username, string password, string organization, string repoID, string baseUrl)
        {
            var httpRequestHandler = new LfdsUsernamePasswordHandler(username, password, organization, repoID, baseUrl);
            return CreateFromHttpRequestHandler(httpRequestHandler, baseUrl);
        }
    }
}
