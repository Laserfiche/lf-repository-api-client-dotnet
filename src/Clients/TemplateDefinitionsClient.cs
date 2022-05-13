using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    partial interface ITemplateDefinitionsClient
    {
        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="templateName">An optional query parameter. Can be used to get a single template definition using the template name.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTemplateDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>, Task<bool>> callback, string repoId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="templateId">The requested template definition ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTemplateFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, Task<bool>> callback, string repoId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="templateName">A required query parameter for the requested template name.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, Task<bool>> callback, string repoId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of template definitions.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>> GetTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of field TemplateFieldInfo.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a collection of matching TemplateFieldInfo by template name.
        /// </summary>
        /// <param name="nextLink">Use nextlink returned from the backend to get the rest of the data.</param>
        /// <param name="maxPageSize">Optionally specify the maximum number of items to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default);
    }

    partial class TemplateDefinitionsClient
    {
        public async Task GetTemplateDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>, Task<bool>> callback, string repoId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateDefinitionsAsync(repoId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetTemplateFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, Task<bool>> callback, string repoId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateFieldDefinitionsAsync(repoId, templateId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateFieldDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, Task<bool>> callback, string repoId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateFieldDefinitionsByTemplateNameAsync(repoId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && await callback(response))
            {
                response = await ApiForEachAsync(_httpClient, response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateFieldDefinitionsByTemplateNameSendAsync, cancellationToken);
            }
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>> GetTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTemplateDefinitionsSendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTemplateFieldDefinitionsSendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(_httpClient, nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetTemplateFieldDefinitionsSendAsync, cancellationToken);
        }
    }
}
