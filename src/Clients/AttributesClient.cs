﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    partial interface IAttributesClient
    {
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get the attribute key value pairs associated with the authenticated user.
        /// </summary>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="everyone">Boolean value that indicates whether to return attributes key value pairs associated with everyone or the currently authenticated user.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTrusteeAttributeKeyValuePairsForEachAsync(Func<SwaggerResponse<ODataValueContextOfListOfAttribute>, bool> callback, string repoId, bool? everyone = null, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of trustee attributes.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfListOfAttribute>> GetTrusteeAttributeKeyValuePairsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    partial class AttributesClient
    {
        public async Task GetTrusteeAttributeKeyValuePairsForEachAsync(Func<SwaggerResponse<ODataValueContextOfListOfAttribute>, bool> callback, string repoId, bool? everyone = null, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTrusteeAttributeKeyValuePairsAsync(repoId, everyone, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTrusteeAttributeKeyValuePairsSendAsync, cancellationToken);
            }
        }

        public async Task<SwaggerResponse<ODataValueContextOfListOfAttribute>> GetTrusteeAttributeKeyValuePairsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTrusteeAttributeKeyValuePairsSendAsync, cancellationToken);
        }
    }
}
