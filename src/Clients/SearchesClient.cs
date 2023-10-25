// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Searches API client.
    /// </summary>
    partial interface ISearchesClient
    {
        /// <summary>
        /// Returns a collection of search results using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListSearchResultsAsync(ListSearchResultsParameters, CancellationToken)">ListSearchResultsAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListSearchResultsForEachAsync(Func<EntryCollectionResponse, Task<bool>> callback, ListSearchResultsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the context hits associated with a search result entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListSearchContextHitsAsync(ListSearchContextHitsParameters, CancellationToken)">ListSearchContextHitsAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListSearchContextHitsForEachAsync(Func<SearchContextHitCollectionResponse, Task<bool>> callback, ListSearchContextHitsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of search results using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListSearchResultsAsync(ListSearchResultsParameters, CancellationToken)">ListSearchResultsAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of entry search results.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<EntryCollectionResponse> ListSearchResultsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the context hits associated with a search result entry using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListSearchContextHitsAsync(ListSearchContextHitsParameters, CancellationToken)">ListSearchContextHitsAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of context hits for a search result.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<SearchContextHitCollectionResponse> ListSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The Laserfiche Repository Searches API client.
    /// </summary>
    partial class SearchesClient
    {
        public async Task ListSearchResultsForEachAsync(Func<EntryCollectionResponse, Task<bool>> callback, ListSearchResultsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListSearchResultsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListSearchResultsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListSearchContextHitsForEachAsync(Func<SearchContextHitCollectionResponse, Task<bool>> callback, ListSearchContextHitsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListSearchContextHitsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListSearchContextHitsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<EntryCollectionResponse> ListSearchResultsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListSearchResultsSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<SearchContextHitCollectionResponse> ListSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListSearchContextHitsSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
