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
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateDefinitionsForEachAsync(Func<TemplateDefinitionCollectionResponse, Task<bool>> callback, ListTemplateDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template id using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateFieldDefinitionsByTemplateIdForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, ListTemplateFieldDefinitionsByTemplateIdParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the field definitions assigned to a template definition by template name using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, ListTemplateFieldDefinitionsByTemplateNameParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        public async Task ListTemplateDefinitionsForEachAsync(Func<TemplateDefinitionCollectionResponse, Task<bool>> callback, ListTemplateDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListTemplateDefinitionsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListTemplateDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListTemplateFieldDefinitionsByTemplateIdForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, ListTemplateFieldDefinitionsByTemplateIdParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListTemplateFieldDefinitionsByTemplateIdAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListTemplateFieldDefinitionsByTemplateIdSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<TemplateFieldDefinitionCollectionResponse, Task<bool>> callback, ListTemplateFieldDefinitionsByTemplateNameParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListTemplateFieldDefinitionsByTemplateNameAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListTemplateFieldDefinitionsByTemplateNameSendAsync, cancellationToken).ConfigureAwait(false);
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
