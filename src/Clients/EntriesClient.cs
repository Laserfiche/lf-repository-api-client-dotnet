using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    partial interface IEntriesClient
    {
        /// <summary>
        /// Get entry with uri.
        /// </summary>
        /// <param name="uriString">Uri string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<Entry>> GetEntryAsync(string uriString, CancellationToken cancellationToken = default);

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
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetEntryListingForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>, bool> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetFieldValuesForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfFieldValue>, bool> callback, string repoId, int entryId, string prefer = null, bool? formatValue = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetLinkValuesFromEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTagsAssignedToEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of entries with odata.nextLink.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of field values.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfFieldValue>> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of WEntryLinkInfo.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of WTagInfo.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    partial class EntriesClient
    {
        public async Task<SwaggerResponse<Entry>> GetEntryAsync(string uriString, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                request.RequestUri = new Uri(uriString, UriKind.Absolute);
                return await GetEntrySendAsync(request, _httpClient, new bool[] { false }, cancellationToken);
            }
        }

        public async Task GetEntryListingForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>, bool> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetEntryListingAsync(repoId, entryId, groupByEntryType, fields, formatFields, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetEntryListingSendAsync, cancellationToken);
            }
        }

        public async Task GetFieldValuesForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfFieldValue>, bool> callback, string repoId, int entryId, string prefer = null, bool? formatValue = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetFieldValuesAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), formatValue, culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetFieldValuesSendAsync, cancellationToken);
            }
        }

        public async Task GetLinkValuesFromEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetLinkValuesFromEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetLinkValuesFromEntrySendAsync, cancellationToken);
            }
        }

        public async Task GetTagsAssignedToEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTagsAssignedToEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTagsAssignedToEntrySendAsync, cancellationToken);
            }
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetEntryListingSendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfFieldValue>> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetFieldValuesSendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetLinkValuesFromEntrySendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTagsAssignedToEntrySendAsync, cancellationToken);
        }
    }
}
