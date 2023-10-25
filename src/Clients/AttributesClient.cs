// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Attributes API client.
    /// </summary>
    partial interface IAttributesClient
    {
        /// <summary>
        /// Returns the attribute key value pairs associated with the authenticated user using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListAttributesAsync(ListAttributesParameters, CancellationToken)">ListAttributesAsync</see>
        /// </remarks>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="parameters">Parameters for the request.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task ListAttributesForEachAsync(Func<AttributeCollectionResponse, Task<bool>> callback, ListAttributesParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the attribute key value pairs associated with the authenticated user using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListAttributesAsync(ListAttributesParameters, CancellationToken)">ListAttributesAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of attributes associated with the authenticated user.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<AttributeCollectionResponse> ListAttributesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The Laserfiche Repository Attributes API client.
    /// </summary>
    partial class AttributesClient
    {
        public async Task ListAttributesForEachAsync(Func<AttributeCollectionResponse, Task<bool>> callback, ListAttributesParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListAttributesAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListAttributesSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<AttributeCollectionResponse> ListAttributesNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListAttributesSendAsync, cancellationToken).ConfigureAwait(false);
        }
    }
}
