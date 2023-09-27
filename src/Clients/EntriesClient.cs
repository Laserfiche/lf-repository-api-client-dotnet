using Laserfiche.Api.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Entries API client.
    /// </summary>
    partial interface IEntriesClient
    {
        /// <summary>
        /// Get entry with uri.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="GetEntryAsync(GetEntryParameters, CancellationToken)">GetEntryAsync</see>
        /// </remarks>
        /// <param name="uriString">Uri string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A single entry.</returns>
        Task<Entry> GetEntryAsync(string uriString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the children entries of a folder in the repository using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListEntriesAsync(ListEntriesParameters, CancellationToken)">ListEntriesAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListEntriesForEachAsync(Func<EntryCollectionResponse, Task<bool>> callback, ListEntriesParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the fields assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListFieldsAsync(ListFieldsParameters, CancellationToken)">ListFieldsAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListFieldsForEachAsync(Func<FieldCollectionResponse, Task<bool>> callback, ListFieldsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the links assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListLinksAsync(ListLinksParameters, CancellationToken)">ListLinksAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListLinksForEachAsync(Func<LinkCollectionResponse, Task<bool>> callback, ListLinksParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the tags assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListTagsAsync(ListTagsParameters, CancellationToken)">ListTagsAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListTagsForEachAsync(Func<TagCollectionResponse, Task<bool>> callback, ListTagsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the children entries of a folder in the repository using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListEntriesAsync(ListEntriesParameters, CancellationToken)">ListEntriesAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of children entries of a folder.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<EntryCollectionResponse> ListEntriesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the fields assigned to an entry using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListFieldsAsync(ListFieldsParameters, CancellationToken)">ListFieldsAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of fields assigned to the entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<FieldCollectionResponse> ListFieldsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the links assigned to an entry using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListLinksAsync(ListLinksParameters, CancellationToken)">ListLinksAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of links assigned to the entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<LinkCollectionResponse> ListLinksNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the tags assigned to an entry using a nextlink
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListTagsAsync(ListTagsParameters, CancellationToken)">ListTagsAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of tags assigned to the entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<TagCollectionResponse> ListTagsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The Laserfiche Repository Entries API client.
    /// </summary>
    partial class EntriesClient
    {
        public async Task<Entry> GetEntryAsync(string uriString, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                request.RequestUri = new Uri(uriString, UriKind.Absolute);
                return await GetEntrySendAsync(request, _httpClient, new bool[] { false }, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListEntriesForEachAsync(Func<EntryCollectionResponse, Task<bool>> callback, ListEntriesParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListEntriesAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListEntriesSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListFieldsForEachAsync(Func<FieldCollectionResponse, Task<bool>> callback, ListFieldsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListFieldsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListFieldsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListLinksForEachAsync(Func<LinkCollectionResponse, Task<bool>> callback, ListLinksParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListLinksAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListLinksSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task ListTagsForEachAsync(Func<TagCollectionResponse, Task<bool>> callback, ListTagsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListTagsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListTagsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<EntryCollectionResponse> ListEntriesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListEntriesSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<FieldCollectionResponse> ListFieldsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListFieldsSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<LinkCollectionResponse> ListLinksNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListLinksSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TagCollectionResponse> ListTagsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListTagsSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
