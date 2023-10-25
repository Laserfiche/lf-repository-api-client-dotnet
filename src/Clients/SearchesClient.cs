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
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="searchToken">The requested searchToken.</param>
        /// <param name="groupByEntryType">An optional query parameter used to indicate if the result should be grouped by entry type or not.</param>
        /// <param name="refresh">If the search listing should be refreshed to show updated values.</param>
        /// <param name="fields">Optional array of field names. Field values corresponding to the given field names will be returned for each search result.</param>
        /// <param name="formatFields">Boolean for if field values should be formatted. Only applicable if Fields are specified.</param>
        /// <param name="prefer">An optional odata header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag. The formatFields query parameter must be set to true, otherwise
        /// <br/>            culture will not be used for formatting.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetSearchResultsForEachAsync(Func<ODataValueContextOfIListOfEntry, Task<bool>> callback, string repoId, string searchToken, bool? groupByEntryType = null, bool? refresh = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the context hits associated with a search result entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="searchToken">The requested searchToken.</param>
        /// <param name="rowNumber">The search result listing row number to get context hits for.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetSearchContextHitsForEachAsync(Func<ODataValueContextOfIListOfContextHit, Task<bool>> callback, string repoId, string searchToken, int rowNumber, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of search results using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get search result successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfEntry> GetSearchResultsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the context hits associated with a search result entry using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get search context hits successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfContextHit> GetSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    partial class SearchesClient
    {
        public async Task GetSearchResultsForEachAsync(Func<ODataValueContextOfIListOfEntry, Task<bool>> callback, string repoId, string searchToken, bool? groupByEntryType = null, bool? refresh = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)

        {
            // Initial request
            var response = await GetSearchResultsAsync(repoId, searchToken, groupByEntryType, refresh, fields, formatFields, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetSearchResultsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task GetSearchContextHitsForEachAsync(Func<ODataValueContextOfIListOfContextHit, Task<bool>> callback, string repoId, string searchToken, int rowNumber, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetSearchContextHitsAsync(repoId, searchToken, rowNumber, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetSearchContextHitsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<ODataValueContextOfIListOfEntry> GetSearchResultsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetSearchResultsSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ODataValueContextOfIListOfContextHit> GetSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetSearchContextHitsSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
