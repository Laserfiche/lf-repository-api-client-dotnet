using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Globalization;

[assembly: InternalsVisibleTo("Laserfiche.Repository.Api.Client.Test")]
namespace Laserfiche.Repository.Api.Client
{
    partial interface ILaserficheRepositoryApiClient
    {
        string AccessToken { get; set; }
        string RefreshToken { get; set; }

        /// <summary>
        /// The headers which should be sent with each request.
        /// </summary>
        System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders { get; }

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
        Task GetEntryListingForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>, bool> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, CancellationToken cancellationToken = default);
 

        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWFieldInfo>, bool> callback, string repoId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        /// <param name="searchToken">The requested searchToken.</param>
        /// <param name="rowNumber">The search result listing row number to get context hits for.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetSearchContextHitsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfContextHit>, bool> callback, string repoId, string searchToken, int rowNumber, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="repoId">The requested repository ID.</param>
        /// <param name="prefer">An optional OData header. Can be used to set the maximum page size using odata.maxpagesize.</param>
        /// <param name="culture">An optional query parameter used to indicate the locale that should be used for formatting.
        /// <br/>            The value should be a standard language tag.</param>
        /// <param name="select">Limits the properties returned in the result.</param>
        /// <param name="orderby">Specifies the order in which items are returned. The maximum number of expressions is 5.</param>
        /// <param name="top">Limits the number of items returned from a collection.</param>
        /// <param name="skip">Excludes the specified number of items of the queried collection from the result.</param>
        /// <param name="count">Indicates whether the total count of items within a collection are returned in the result.</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetTagDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>, bool> callback, string repoId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        Task GetTemplateDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>, bool> callback, string repoId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        Task GetTemplateFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, bool> callback, string repoId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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
        Task GetTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, bool> callback, string repoId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

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

        /// <param name="callback">A delegate that will be called each time new data is retrieved. Returns false to stop receiving more data; returns true to be called again if there's more data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get the search results listing of a search.
        /// </summary>
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
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task GetSearchResultsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataEntry>, bool> callback, string repoId, string searchToken, bool? groupByEntryType = null, bool? refresh = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfODataEntry>> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfWFieldInfo>> GetFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfFieldValue>> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfContextHit>> GetSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfODataEntry>> GetSearchResultsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>> GetTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);

        Task<SwaggerResponse<ODataValueContextOfListOfAttribute>> GetTrusteeAttributeKeyValuePairsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default);
    }

    partial class LaserficheRepositoryApiClient : ILaserficheRepositoryApiClient
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _httpClient.DefaultRequestHeaders; }
        }

        public async Task<SwaggerResponse<T>> ApiForEachAsync<T>(string nextLink, string prefer, Func<HttpRequestMessage, HttpClient, bool[], CancellationToken, Task<SwaggerResponse<T>>> sendAndProcessResponseAsync, CancellationToken cancellationToken) where T : new()
        {
            if (nextLink == null)
            {
                return null;
            }
            else
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = new HttpMethod("GET");
                    request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    if (prefer != null)
                    {
                        request.Headers.TryAddWithoutValidation("Prefer", ConvertToString(prefer, CultureInfo.InvariantCulture));
                    }
                    request.RequestUri = new Uri(nextLink, UriKind.Absolute);

                    var response = await sendAndProcessResponseAsync(request, _httpClient, new bool[] { false }, default);
                    return response;
                }
            }
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfODataEntry>> GetEntryListingNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            return await ApiForEachAsync(nextLink, MergeMaxSizeIntoPrefer(maxPageSize, null), GetEntryListingSendAsync, cancellationToken);
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWFieldInfo>> GetFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfFieldValue>> GetFieldValuesNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>> GetLinkValuesFromEntryNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfContextHit>> GetSearchContextHitsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfODataEntry>> GetSearchResultsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>> GetTagsAssignedToEntryNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>> GetTemplateDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>> GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SwaggerResponse<ODataValueContextOfListOfAttribute>> GetTrusteeAttributeKeyValuePairsNextLinkAsync(string nextLink, int? maxPageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task GetEntryListingForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataEntry>, bool> callback, string repoId, int entryId, bool? groupByEntryType = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetEntryListingAsync(repoId, entryId, groupByEntryType, fields, formatFields, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);
            
            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetEntryListingSendAsync, cancellationToken);
            }
        }

        public async Task GetFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWFieldInfo>, bool> callback, string repoId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetFieldDefinitionsAsync(repoId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetFieldDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetFieldValuesForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfFieldValue>, bool> callback, string repoId, int entryId, string prefer = null, bool? formatValue = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetFieldValuesAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), formatValue, culture, select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetFieldValuesSendAsync, cancellationToken);
            }
        }

        public async Task GetLinkValuesFromEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetLinkValuesFromEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetLinkValuesFromEntrySendAsync, cancellationToken);
            }
        }

        public async Task GetSearchContextHitsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfContextHit>, bool> callback, string repoId, string searchToken, int rowNumber, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetSearchContextHitsAsync(repoId, searchToken, rowNumber, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetSearchContextHitsSendAsync, cancellationToken);
            }
        }

        public async Task GetTagDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>, bool> callback, string repoId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTagDefinitionsAsync(repoId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTagDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetTagsAssignedToEntryForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTagInfo>, bool> callback, string repoId, int entryId, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTagsAssignedToEntryAsync(repoId, entryId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTagsAssignedToEntrySendAsync, cancellationToken);
            }
        }

        public async Task GetTemplateDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo>, bool> callback, string repoId, string templateName = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateDefinitionsAsync(repoId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetTemplateFieldDefinitionsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, bool> callback, string repoId, int templateId, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateFieldDefinitionsAsync(repoId, templateId, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateFieldDefinitionsSendAsync, cancellationToken);
            }
        }

        public async Task GetTemplateFieldDefinitionsByTemplateNameForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo>, bool> callback, string repoId, string templateName, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTemplateFieldDefinitionsByTemplateNameAsync(repoId, templateName, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTemplateFieldDefinitionsByTemplateNameSendAsync, cancellationToken);
            }
        }

        public async Task GetTrusteeAttributeKeyValuePairsForEachAsync(Func<SwaggerResponse<ODataValueContextOfListOfAttribute>, bool> callback, string repoId, bool? everyone = null, string prefer = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, int? maxPageSize = null, CancellationToken cancellationToken = default)
        {
            // Initial request
            var response = await GetTrusteeAttributeKeyValuePairsAsync(repoId, everyone, MergeMaxSizeIntoPrefer(maxPageSize, prefer), select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetTrusteeAttributeKeyValuePairsSendAsync, cancellationToken);
            }
        }


        public async Task GetSearchResultsForEachAsync(Func<SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren>, bool> callback, string repoId, string searchToken, bool? groupByEntryType = null, bool? refresh = null, IEnumerable<string> fields = null, bool? formatFields = null, string prefer = null, string culture = null, string select = null, string orderby = null, int? top = null, int? skip = null, bool? count = null, CancellationToken cancellationToken = default)

        {
            // Initial request
            var response = await GetSearchResultsAsync(repoId, searchToken, groupByEntryType, refresh, fields, formatFields, MergeMaxSizeIntoPrefer(maxPageSize, prefer), culture, select, orderby, top, skip, count, cancellationToken);
            var result = response.Result;

            // Further requests
            while (!cancellationToken.IsCancellationRequested && response != null && callback(response))
            {
                response = await ApiForEachAsync(response.Result.OdataNextLink, MergeMaxSizeIntoPrefer(maxPageSize, prefer), GetSearchResultsSendAsync, cancellationToken);
            }
        }

        /// <summary>
        /// Get entry with redirect url. If url validation fail, it will throw exception.
        /// </summary>
        /// <param name="uriString">Redirect url string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        public async Task<SwaggerResponse<Entry>> GetEntryAsync(string uriString, CancellationToken cancellationToken = default)
        {
            string repoIdKey = "{repoId}";
            string entryIdKey = "{entryId}";
            string selectKey = "{select}";
            string templateUriString = $"{_httpClient.BaseAddress}v1/Repositories/{repoIdKey}/Entries/{entryIdKey}?$select={selectKey}";
            Uri templateUri = new Uri(templateUriString);
            Uri uri = new Uri(uriString);

            var paramDict = ValidateAndGetParamtersFromUri(templateUri, uri);
            string repoId = paramDict[repoIdKey];
            int entryId = 0;
            if (!Int32.TryParse(paramDict[entryIdKey], out entryId))
            {
                throw new ArgumentException($"Invalid value {paramDict[entryIdKey]} for entryId.");
            }
            string select = paramDict[selectKey];
            return await GetEntryAsync(repoId, entryId, select, cancellationToken);
        }

        private string MergeMaxSizeIntoPrefer(int? maxSize, string prefer)
        {
            if (maxSize == null)
            {
                return prefer;
            }
            else if (prefer == null)
            {
                return string.Format("maxpagesize={0}", maxSize);
            }
            else // Prefer's format: https://tools.ietf.org/id/draft-snell-http-prefer-16.html#prefer
            {
                // Based on prefer's format, we can just append maxpagesize
                return prefer + string.Format("; maxpagesize={0}", maxSize);
            }
        }

        /// <summary>
        /// Validate redirect uri against template uri to check if host and segment path all match and there is no extra unecessary query parameters.
        /// If it passes the validation, the corresponding parameter token and values will return in a dictionary.
        /// </summary>
        /// <param name="templateUri">the standard template uri with parameter token like "{parameterName}"</param>
        /// <param name="redirectUri">the redirect uri needs to be validated and get parameters from</param>
        /// <returns></returns>
        internal Dictionary<string, string> ValidateAndGetParamtersFromUri(Uri templateUri, Uri redirectUri)
        {
            if (templateUri == null || redirectUri == null)
            {
                throw new ArgumentException($"uris cannot be null");
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            // check host
            if (!templateUri.Host.Equals(redirectUri.Host))
            {
                throw new ArgumentException($"url host {redirectUri.Host} not match client's host {templateUri.Host}");
            }

            // check segments
            var templateSegments = templateUri.Segments.Select(item => HttpUtility.UrlDecode(item.TrimEnd('/'))).ToArray();
            var redirectSegments = redirectUri.Segments.Select(item => HttpUtility.UrlDecode(item.TrimEnd('/'))).ToArray();

            int i = 0, j = 0;
            for (; i < templateSegments.Length && j < redirectSegments.Length; i++, j++)
            {
                if (string.IsNullOrEmpty(templateSegments[i]))
                {
                    // skip empty segment
                    continue;
                }

                if (templateSegments[i].StartsWith("{") && templateSegments[i].EndsWith("}"))
                {
                    // found parameter segment
                    if (i > 0)
                    {
                        if (templateSegments[i - 1].Equals(redirectSegments[j - 1], StringComparison.OrdinalIgnoreCase))
                        {
                            result[templateSegments[i]] = redirectSegments[j];
                        }
                        else
                        {
                            if (redirectSegments[j].StartsWith(templateSegments[i - 1], StringComparison.OrdinalIgnoreCase))
                            {
                                string value = redirectSegments[j].Substring(templateSegments[i - 1].Length, redirectSegments[j].Length - templateSegments[i - 1].Length);
                                value = value.TrimStart('(');
                                value = value.TrimEnd(')');
                                value = value.Trim('\'');
                                result[templateSegments[i]] = value;

                                continue;
                            }
                            throw new ArgumentException($"Invalid url. url should be like {templateUri}");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid template url. Parameter should not be in first segment.");
                    }
                }
                else
                {
                    // compare regular segment
                    if (!templateSegments[i].Equals(redirectSegments[j], StringComparison.OrdinalIgnoreCase)
                        && !redirectSegments[j].StartsWith(templateSegments[i], StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException($"Invalid url. url should be like {templateUri}");
                    }
                    if (!templateSegments[i].Equals(redirectSegments[j], StringComparison.OrdinalIgnoreCase)
                        && redirectSegments[j].StartsWith(templateSegments[i], StringComparison.OrdinalIgnoreCase))
                    {
                        j--; // not to increase j, if regular segment not completely the same
                    }
                }
            }
            if ((i == templateSegments.Length && j < redirectSegments.Length) || 
                (j == redirectSegments.Length && i < templateSegments.Length))
            {
                throw new ArgumentException($"Invalid url. url should be like {templateUri}");
            }


            // check queries
            var redirectQueries = HttpUtility.ParseQueryString(redirectUri.Query);
            var templateQueries = HttpUtility.ParseQueryString(templateUri.Query);
            foreach (var redirectQueryKey in redirectQueries.AllKeys)
            {
                if (templateQueries.AllKeys.Contains(redirectQueryKey))
                {
                    result[templateQueries[redirectQueryKey]] = redirectQueries[redirectQueryKey];
                }
                else
                {
                    throw new ArgumentException($"{redirectQueryKey} is not supported query in {templateUri.Query}");
                }
            }
            foreach (var templateQuery in templateQueries.AllKeys)
            {
                var templateQueryToken = templateQueries[templateQuery];
                if (!result.ContainsKey(templateQueryToken))
                {
                    result[templateQueryToken] = null;
                }
            }

            return result;
        }
    }

    partial class PostEntryWithEdocMetadataRequest : IFormattable
    {
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider is System.Globalization.CultureInfo cultureInfo)
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Culture = cultureInfo });
            }
            return JsonConvert.SerializeObject(this);
        }
    }

    #region inheritance
    [JsonConverter(typeof(JsonInheritanceConverter), "entryType")]
    [JsonInheritance("Document", typeof(Document))]
    [JsonInheritance("Folder", typeof(Folder))]
    [JsonInheritance("Shortcut", typeof(Shortcut))]
    partial class Entry
    { 
    }

    // JsonInheritanceAttribute and JsonInheritanceConverter are NSwag auto generated code using the example swagger schema here https://github.com/RicoSuter/NJsonSchema/wiki/Inheritance
    // so client lib don't need to add dependency to NJsonSchema
    // we don't add "discriminator" at server side because it will add another property in json schema and we already have @odata.type and entryType could be used for discrimination
    // only need JsonInheritanceAttribute and JsonInheritanceConverter on client side for response deserialize
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v11.0.0.0)")]
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    internal class JsonInheritanceAttribute : System.Attribute
    {
        public JsonInheritanceAttribute(string key, System.Type type)
        {
            Key = key;
            Type = type;
        }

        public string Key { get; }

        public System.Type Type { get; }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v11.0.0.0)")]
    internal class JsonInheritanceConverter : Newtonsoft.Json.JsonConverter
    {
        internal static readonly string DefaultDiscriminatorName = "discriminator";

        private readonly string _discriminator;

        [System.ThreadStatic]
        private static bool _isReading;

        [System.ThreadStatic]
        private static bool _isWriting;

        public JsonInheritanceConverter()
        {
            _discriminator = DefaultDiscriminatorName;
        }

        public JsonInheritanceConverter(string discriminator)
        {
            _discriminator = discriminator;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                _isWriting = true;

                var jObject = Newtonsoft.Json.Linq.JObject.FromObject(value, serializer);
                try
                {
                    jObject.AddFirst(new Newtonsoft.Json.Linq.JProperty(_discriminator, GetSubtypeDiscriminator(value.GetType())));
                }
                catch (Exception) { }
                writer.WriteToken(jObject.CreateReader());
            }
            finally
            {
                _isWriting = false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (_isWriting)
                {
                    _isWriting = false;
                    return false;
                }
                return true;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_isReading)
                {
                    _isReading = false;
                    return false;
                }
                return true;
            }
        }

        public override bool CanConvert(System.Type objectType)
        {
            return true;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(reader);
            if (jObject == null)
                return null;

            var discriminatorValue = jObject.GetValue(_discriminator);
            var discriminator = discriminatorValue != null ? Newtonsoft.Json.Linq.Extensions.Value<string>(discriminatorValue) : null;
            var subtype = GetObjectSubtype(objectType, discriminator);

            var objectContract = serializer.ContractResolver.ResolveContract(subtype) as Newtonsoft.Json.Serialization.JsonObjectContract;
            if (objectContract == null || System.Linq.Enumerable.All(objectContract.Properties, p => p.PropertyName != _discriminator))
            {
                jObject.Remove(_discriminator);
            }

            try
            {
                _isReading = true;
                return serializer.Deserialize(jObject.CreateReader(), subtype);
            }
            finally
            {
                _isReading = false;
            }
        }

        private System.Type GetObjectSubtype(System.Type objectType, string discriminator)
        {
            foreach (var attribute in System.Reflection.CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(System.Reflection.IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Key == discriminator)
                    return attribute.Type;
            }

            return objectType;
        }

        private string GetSubtypeDiscriminator(System.Type objectType)
        {
            foreach (var attribute in System.Reflection.CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(System.Reflection.IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Type == objectType)
                    return attribute.Key;
            }

            return objectType.Name;
        }
    }
    #endregion
}
