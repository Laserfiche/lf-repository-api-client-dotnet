namespace Laserfiche.Repository.Api.Client
{
    public interface IRepositoryApiClient
    {
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
}
