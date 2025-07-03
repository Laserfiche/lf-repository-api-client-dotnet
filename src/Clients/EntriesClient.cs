// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using System;
using System.Diagnostics;
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

        /// <summary>
        /// Updates the field values assigned to an entry. Can retry if entry is locked.
        /// </summary>
        /// <remarks>
        /// - Update the field values assigned to an entry.<br/>
        /// - Provide the new field values to assign to the entry, and remove/reset all previously assigned field values.<br/>
        /// - This is an overwrite action. The request body must include all desired field values, including any existing field values that should remain assigned to the entry. Field values that are not included in the request will be deleted from the entry. If the field value that is not included is part of a template, it will still be assigned (as required by the template), but its value will be reset.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of fields assigned to the entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<FieldCollectionResponse> SetFieldsAsync(SetFieldsParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns tags to an entry. Can retry if entry is locked.
        /// </summary>
        /// <remarks>
        /// - Assign tags to an entry.<br/>
        /// - Provide an entry ID and a list of tags to assign to that entry.<br/>
        /// - This is an overwrite action. The request must include all tags to assign to the entry, including existing tags that should remain assigned to the entry.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of tags assigned to the entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<TagCollectionResponse> SetTagsAsync(SetTagsParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns a template to an entry. Can retry if entry is locked.
        /// </summary>
        /// <remarks>
        /// - Assign a template to an entry.<br/>
        /// - Provide an entry ID, template name, and a list of template fields to assign to that entry.<br/>
        /// - Only template values will be modified. Any existing independent fields on the entry will not be modified, nor will they be added if included in the request. The only modification to fields will only occur on templated fields. If the previously assigned template includes common template fields as the newly assigned template, the common field values will not be modified.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The updated entry.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<Entry> SetTemplateAsync(SetTemplateParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// The Laserfiche Repository Entries API client.
    /// </summary>
    partial class EntriesClient
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

        public async Task<Entry> SetTemplateAsync(SetTemplateParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default)
        {
            Func<Task<Entry>> setTemplateResponse = async () =>
            {
                return await SetTemplateAsync(parameters, cancellationToken);
            };
            Entry entryAfterSetTemplate = await RetryEntryOperationIfLocked(setTemplateResponse, (TimeSpan)retryIfLockedFor);
            return entryAfterSetTemplate;
        }

        public async Task<FieldCollectionResponse> SetFieldsAsync(SetFieldsParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default)
        {
            Func<Task<FieldCollectionResponse>> trySetFields = async () =>
            {
                return await SetFieldsAsync(parameters, cancellationToken);
            };
            FieldCollectionResponse setFieldsResponse = await RetryEntryOperationIfLocked(trySetFields, (TimeSpan)retryIfLockedFor);
            return setFieldsResponse;
        }

        public async Task<TagCollectionResponse> SetTagsAsync(SetTagsParameters parameters, TimeSpan retryIfLockedFor, CancellationToken cancellationToken = default)
        {
            Func<Task<TagCollectionResponse>> trySetTags = async () =>
            {
                return await SetTagsAsync(parameters, cancellationToken);
            };
            TagCollectionResponse setTagsResponse = await RetryEntryOperationIfLocked(trySetTags, (TimeSpan)retryIfLockedFor);
            return setTagsResponse;
        }

        private static async Task<T> RetryEntryOperationIfLocked<T>(Func<Task<T>> performAction, TimeSpan retryIfLockedFor)
        {
            bool retryCall = true;
            Stopwatch sw = Stopwatch.StartNew();
            T entry = default(T);
            while (sw.Elapsed < retryIfLockedFor && retryCall)
            {
                try
                {
                    entry = await performAction();
                    retryCall = false;
                }
                catch (ApiException ex)
                {
                    if (ex.StatusCode != 423)
                    {
                        throw;
                    }
                }
            }
            return entry;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
