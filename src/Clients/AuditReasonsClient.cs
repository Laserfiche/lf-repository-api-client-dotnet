// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    partial interface IAuditReasonsClient
    {
        /// <summary>
        /// Returns the attribute key value pairs associated with the authenticated user using a nextlink.
        /// </summary>
        /// <remarks>
        /// - Related: <see cref="ListAuditReasonsAsync(ListAuditReasonsParameters, CancellationToken)">ListAttributesAsync</see>
        /// </remarks>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of attributes associated with the authenticated user.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<AuditReasonCollectionResponse> ListAuditReasonsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// The Laserfiche Repository Audit Reasons API client.
    /// </summary>
    partial class AuditReasonsClient
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public async Task ListAuditReasonsForEachAsync(Func<AuditReasonCollectionResponse, Task<bool>> callback, ListAuditReasonsParameters parameters, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            parameters.Prefer = MergeMaxSizeIntoPrefer(maxPageSize, parameters.Prefer);
            var response = await ListAuditReasonsAsync(parameters, cancellationToken).ConfigureAwait(false);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response).ConfigureAwait(false))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, parameters.Prefer, ListAuditReasonsSendAsync, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<AuditReasonCollectionResponse> ListAuditReasonsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), ListAuditReasonsSendAsync, cancellationToken).ConfigureAwait(false);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
