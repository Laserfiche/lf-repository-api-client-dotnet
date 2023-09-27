using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Field Definitions API client.
    /// </summary>
    partial interface IFieldDefinitionsClient
    {
        /// <summary>
        /// Returns a collection of field definitions using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListFieldDefinitionsForEachAsync(Func<FieldDefinitionCollectionResponse, Task<bool>> callback, ListFieldDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of field definitions using a nextlink. 
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of field definitions.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<FieldDefinitionCollectionResponse> ListFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// The Laserfiche Repository Field Definitions API client.
    /// </summary>
    partial class FieldDefinitionsClient
    {
        public async Task ListFieldDefinitionsForEachAsync(Func<FieldDefinitionCollectionResponse, Task<bool>> callback, ListFieldDefinitionsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListFieldDefinitionsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListFieldDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<FieldDefinitionCollectionResponse> ListFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListFieldDefinitionsSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
