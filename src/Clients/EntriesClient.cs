// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
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
        /// <param name="uriString">Uri string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get entry successfully.</returns>
        Task<Entry> GetEntryAsync(string uriString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the children entries of a folder in the repository using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The folder ID.</param>
        /// <param name="groupByEntryType">An optional query parameter used to indicate if the result should be grouped by entry type or not.</param>
        /// <param name="fields">Optional array of field names. Field values corresponding to the given field names will be returned for each entry.</param>
        /// <param name="formatFields">Boolean for if field values should be formatted. Only applicable if Fields are specified.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
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
        Task GetEntryListingForEachAsync(Func<ODataValueContextOfIListOfEntry, Task<bool>> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the fields assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The requested entry ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="formatValue">An optional query parameter used to indicate if the field values should be formatted.
        /// <br/>            The default value is false.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag. The formatValue query parameter must be set to true, otherwise
        /// <br/>            culture will not be used for formatting.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetFieldValuesForEachAsync(Func<ODataValueContextOfIListOfFieldValue, Task<bool>> callback, string repoId, int entryId, string prefer = null, bool? formatValue = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the links assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The requested entry ID.</param>
        /// <param name="prefer">An optional odata header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetLinkValuesFromEntryForEachAsync(Func<ODataValueContextOfIListOfWEntryLinkInfo, Task<bool>> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the tags assigned to an entry using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The requested entry ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTagsAssignedToEntryForEachAsync(Func<ODataValueContextOfIListOfWTagInfo, Task<bool>> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the children entries of a folder in the repository using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get the children entries of a Folder successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfEntry> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the fields assigned to an entry using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get field values successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfFieldValue> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the links assigned to an entry using a nextlink.
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get links successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfWEntryLinkInfo> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the tags assigned to an entry using a nextlink
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get entry tags successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfWTagInfo> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates/copies a new child entry in a folder. Allows retry if entry is locked
        /// </summary>
        /// <remarks>
        /// - Create/copy a new child entry in the designated folder.<br/>
        /// - Provide the parent folder ID, and based on the request body, copy or create a folder/shortcut as a child entry of the designated folder.<br/>
        /// - Optional parameter: autoRename (default false). If an entry already exists with the given name, the entry will be automatically renamed.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The folder ID that the entry will be created in.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="request">The entry to create.</param>
        /// <param name="autoRename">An optional query parameter used to indicate if the new entry should be automatically renamed if an entry already exists with the given name in the folder. The default value is false.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used. The value should be a standard language tag.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Created a new child entry successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<Entry> CreateOrCopyEntryAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PostEntryChildrenRequest request = null, bool? autoRename = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the field values assigned to an entry. Allows retry if entry is locked
        /// </summary>
        /// <remarks>
        /// - Update the field values assigned to an entry.<br/>
        /// - Provide the new field values to assign to the entry, and remove/reset all previously assigned field values.<br/>
        /// - This is an overwrite action. The request body must include all desired field values, including any existing field values that should remain assigned to the entry. Field values that are not included in the request will be deleted from the entry. If the field value that is not included is part of a template, it will still be assigned (as required by the template), but its value will be reset.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The entry ID of the entry that will have its fields updated.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used. The value should be a standard language tag. This may be used when setting field values with tokens.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Update field values successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueOfIListOfFieldValue> AssignFieldValuesAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, IDictionary<string, FieldToUpdate> fieldsToUpdate = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Assigns tags to an entry. Allows retry if entry is locked
        /// </summary>
        /// <remarks>
        /// - Assign tags to an entry.<br/>
        /// - Provide an entry ID and a list of tags to assign to that entry.<br/>
        /// - This is an overwrite action. The request must include all tags to assign to the entry, including existing tags that should remain assigned to the entry.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The requested entry ID.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="tagsToAdd">The tags to add.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Assign tags to an entry successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueOfIListOfWTagInfo> AssignTagsAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PutTagRequest tagsToAdd = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns links to an entry. Allows retry if entry is locked
        /// </summary>
        /// <remarks>
        /// - Assign links to an entry.<br/>
        /// - Provide an entry ID and a list of links to assign to that entry.<br/>
        /// - This is an overwrite action. The request must include all links to assign to the entry, including existing links that should remain assigned to the entry.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="repoId">The request repository ID.</param>
        /// <param name="entryId">The requested entry ID.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="linksToAdd">Links to add</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Assign links to an entry successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueOfIListOfWEntryLinkInfo> AssignEntryLinksAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, IEnumerable<PutLinksRequest> linksToAdd = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns a template to an entry. Allows retry if entry is locked
        /// </summary>
        /// <remarks>
        /// - Assign a template to an entry.<br/>
        /// - Provide an entry ID, template name, and a list of template fields to assign to that entry.<br/>
        /// - Only template values will be modified. Any existing independent fields on the entry will not be modified, nor will they be added if included in the request. The only modification to fields will only occur on templated fields. If the previously assigned template includes common template fields as the newly assigned template, the common field values will not be modified.<br/>
        /// - Required OAuth scope: repository.Write
        /// </remarks>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="entryId">The ID of entry that will have its template updated.</param>
        /// <param name="retryIfLockedFor">If passed, the client will retry if the entry is locked, until it is no longer locked or the timeout is reached.</param>
        /// <param name="request">The template and template fields that will be assigned to the entry.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used. The value should be a standard language tag. This may be used when setting field values with tokens.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Assign a template successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<Entry> WriteTemplateValueToEntryAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PutTemplateRequest request = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken));
    }

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

        public async Task GetEntryListingForEachAsync(Func<ODataValueContextOfIListOfEntry, Task<bool>> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetEntryListingAsync(repoId, entryId, groupByEntryType, fields, formatFields, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetEntryListingSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task GetFieldValuesForEachAsync(Func<ODataValueContextOfIListOfFieldValue, Task<bool>> callback, string repoId, int entryId, string prefer = null, bool? formatValue = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetFieldValuesAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), formatValue, culture, select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetFieldValuesSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task GetLinkValuesFromEntryForEachAsync(Func<ODataValueContextOfIListOfWEntryLinkInfo, Task<bool>> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetLinkValuesFromEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetLinkValuesFromEntrySendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task GetTagsAssignedToEntryForEachAsync(Func<ODataValueContextOfIListOfWTagInfo, Task<bool>> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTagsAssignedToEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTagsAssignedToEntrySendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<ODataValueContextOfIListOfEntry> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetEntryListingSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ODataValueContextOfIListOfFieldValue> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetFieldValuesSendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ODataValueContextOfIListOfWEntryLinkInfo> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetLinkValuesFromEntrySendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ODataValueContextOfIListOfWTagInfo> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTagsAssignedToEntrySendAsync, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Entry> CreateOrCopyEntryAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PostEntryChildrenRequest request = null, bool? autoRename = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<Entry>> tryCreateEntry = async () =>
            {
                return await CreateOrCopyEntryAsync(repoId, entryId, request, autoRename, culture, cancellationToken);
            };
            Entry createEntryResponse = await RetryEntryOperationIfLocked(tryCreateEntry, (TimeSpan)retryIfLockedFor);
            return createEntryResponse;
        }

        public async Task<ODataValueOfIListOfFieldValue> AssignFieldValuesAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, IDictionary<string, FieldToUpdate> fieldsToUpdate = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<ODataValueOfIListOfFieldValue>> tryAssignFields = async () =>
            {
                return await AssignFieldValuesAsync(repoId, entryId, fieldsToUpdate, culture, cancellationToken);
            };
            ODataValueOfIListOfFieldValue assignFieldValuesResponse = await RetryEntryOperationIfLocked(tryAssignFields, (TimeSpan)retryIfLockedFor);
            return assignFieldValuesResponse;
        }


        public async Task<ODataValueOfIListOfWTagInfo> AssignTagsAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PutTagRequest tagsToAdd = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<ODataValueOfIListOfWTagInfo>> tryAssignTags = async () =>
            {
                return await AssignTagsAsync(repoId, entryId, tagsToAdd, cancellationToken);
            };
            ODataValueOfIListOfWTagInfo assignTagsResponse = await RetryEntryOperationIfLocked(tryAssignTags, (TimeSpan)retryIfLockedFor);
            return assignTagsResponse;
        }


        public async Task<ODataValueOfIListOfWEntryLinkInfo> AssignEntryLinksAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, IEnumerable<PutLinksRequest> linksToAdd = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<ODataValueOfIListOfWEntryLinkInfo>> tryAssignLinks = async () =>
            {
                return await AssignEntryLinksAsync(repoId, entryId, linksToAdd, cancellationToken);
            };
            ODataValueOfIListOfWEntryLinkInfo assignLinksResponse = await RetryEntryOperationIfLocked(tryAssignLinks, (TimeSpan)retryIfLockedFor);
            return assignLinksResponse;
        }


        public async Task<Entry> WriteTemplateValueToEntryAsync(string repoId, int entryId, TimeSpan retryIfLockedFor, PutTemplateRequest request = null, string culture = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<Entry>> tryWriteTemplate = async () =>
            {
                return await WriteTemplateValueToEntryAsync(repoId, entryId, request, culture, cancellationToken);
            };
            Entry writeTemplateResponse = await RetryEntryOperationIfLocked(tryWriteTemplate, (TimeSpan)retryIfLockedFor);
            return writeTemplateResponse;
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
                    string LockErrorCode = "[9014]";
                    string EntrySharingErrorCode = "[9059]";
                    if (ex.StatusCode != 423 && !ex.ProblemDetails.Title.Contains(EntrySharingErrorCode) && !ex.ProblemDetails.Title.Contains(LockErrorCode))
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
