using System.Net.Http.Headers;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository API client.
    /// </summary>
    public interface IRepositoryApiClient
    {
        /// <summary>
        /// The headers which will be sent with each request.
        /// </summary>
        HttpRequestHeaders DefaultRequestHeaders { get; }

        /// <summary>
        /// The Laserfiche Repository Attributes API client.
        /// </summary>
        IAttributesClient AttributesClient { get; }
        /// <summary>
        /// The Laserfiche Repository Audit Reasons API client.
        /// </summary>
        IAuditReasonsClient AuditReasonsClient { get; }
        /// <summary>
        /// The Laserfiche Repository Entries API client.
        /// </summary>
        IEntriesClient EntriesClient { get; }
        /// <summary>
        /// The Laserfiche Repository Field Definitions API client.
        /// </summary>
        IFieldDefinitionsClient FieldDefinitionsClient { get; }
        /// <summary>
        /// The Laserfiche Repository Link Definitions API client.
        /// </summary>
        ILinkDefinitionsClient LinkDefinitionsClient { get; }
        /// <summary>
        /// The Laserfiche Repository Repositories API client.
        /// </summary>
        IRepositoriesClient RepositoriesClient { get; }
        /// <summary>
        /// The Laserfiche Repository Searches API client.
        /// </summary>
        ISearchesClient SearchesClient { get; }
        /// <summary>
        /// The Laserfiche Repository Simple Searches API client.
        /// </summary>
        ISimpleSearchesClient SimpleSearchesClient { get; }
        /// <summary>
        /// The Laserfiche Repository Tag Definitions API client.
        /// </summary>
        ITagDefinitionsClient TagDefinitionsClient { get; }
        /// <summary>
        /// The Laserfiche Repository Tasks API client.
        /// </summary>
        ITasksClient TasksClient { get; }
        /// <summary>
        /// The Laserfiche Repository Template Definitions API client.
        /// </summary>
        ITemplateDefinitionsClient TemplateDefinitionsClient { get; }
    }
}
