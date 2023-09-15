using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Template Definitions API client.
    /// </summary>
    partial interface ITemplateDefinitionsClient
    {
        /// <summary>
        /// Returns a collection of template definitions using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repositoryId">The requested repository ID.</param>
        /// <param name="templateName">An optional query parameter. Can be used to get a single template definition using the template name.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateDefinitionsForEachAsync(Func<TemplateDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template id using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repositoryId">The requested repository ID.</param>
        /// <param name="templateId">The requested template definition ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateFieldDefinitionsByTemplateIdForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template name using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repositoryId">The requested repository ID.</param>
        /// <param name="templateName">A required query parameter for the requested template name.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of template definitions using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get template definitions successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<TemplateDefinitionCollectionResponse> ListTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template id using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get template field definitions successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<TemplateFieldDefinitionCollectionResponse> ListTemplateFieldDefinitionsByTemplateIdNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template name using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get template field definitions successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<TemplateFieldDefinitionCollectionResponse> ListTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    partial class TemplateDefinitionsClient
    {
        public async Task ListTemplateDefinitionsForEachAsync(Func<TemplateDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await ListTemplateDefinitionsAsync(repositoryId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), ListTemplateDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListTemplateFieldDefinitionsByTemplateIdForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await ListTemplateFieldDefinitionsByTemplateIdAsync(repositoryId, templateId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), ListTemplateFieldDefinitionsByTemplateIdSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, string repositoryId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await ListTemplateFieldDefinitionsByTemplateNameAsync(repositoryId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), ListTemplateFieldDefinitionsByTemplateNameSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<TemplateDefinitionCollectionResponse> ListTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListTemplateDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TemplateFieldDefinitionCollectionResponse> ListTemplateFieldDefinitionsByTemplateIdNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListTemplateFieldDefinitionsByTemplateIdSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TemplateFieldDefinitionCollectionResponse> ListTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListTemplateFieldDefinitionsByTemplateIdSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
