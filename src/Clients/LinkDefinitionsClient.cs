using Laserfiche.Api.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// The Laserfiche Repository Link Definitions API client.
    /// </summary>
    partial interface ILinkDefinitionsClient
    {
        /// <summary>
        /// Returns a collection of link definitions using paging. Page results are returned to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetLinkDefinitionsForEachAsync(Func<ODataValueContextOfIListOfEntryLinkTypeInfo, Task<bool>> callback, string repoId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a collection of link definitions using a nextlink. 
        /// </summary>
        /// <param name="nextLink">A url that allows retrieving the next subset of the requested collection.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Get link definitions successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<ODataValueContextOfIListOfEntryLinkTypeInfo> GetLinkDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

    }

    partial class LinkDefinitionsClient
    {
        public async Task GetLinkDefinitionsForEachAsync(Func<ODataValueContextOfIListOfEntryLinkTypeInfo, Task<bool>> callback, string repoId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetLinkDefinitionsAsync(repoId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response))
            {
                response = await GetNextLinkAsync(_httpClient, response.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetLinkDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task<ODataValueContextOfIListOfEntryLinkTypeInfo> GetLinkDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await GetNextLinkAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetLinkDefinitionsSendAsync, cancellationToken);
        }
    }
}
