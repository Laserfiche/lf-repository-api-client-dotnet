using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Link Definitions API client.
    /// </summary>
    partial interface ILinkDefinitionsClient
    {
        /// <summary>
        /// Returns a collection of link definitions using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListLinkDefinitionsForEachAsync(Func<LinkDefinitionCollectionResponse, Task<bool>> callback, ListLinkDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of link definitions using a nextlink. 
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of link definitions.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<LinkDefinitionCollectionResponse> ListLinkDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// The Laserfiche Repository Link Definitions API client.
    /// </summary>
    partial class LinkDefinitionsClient
    {
        public async Task ListLinkDefinitionsForEachAsync(Func<LinkDefinitionCollectionResponse, Task<bool>> callback, ListLinkDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListLinkDefinitionsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListLinkDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<LinkDefinitionCollectionResponse> ListLinkDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListLinkDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
