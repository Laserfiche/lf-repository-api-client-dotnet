// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Laserfiche.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    #region Interface

    partial interface IEntriesClient
    {
        /// <summary>Creates a persistent lock on a document.</summary>
        Task<LockInfo> LockDocumentAsync(LockDocumentParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Gets the persistent lock state of a document.</summary>
        Task<LockInfo> GetDocumentLockInfoAsync(GetDocumentLockInfoParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Removes a persistent lock from a document. Pass LockToken to unlock another user's lock.</summary>
        Task UnlockDocumentAsync(UnlockDocumentParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Puts a document under version control.</summary>
        Task<Entry> PutUnderVersionControlAsync(PutUnderVersionControlParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Checks out a document.</summary>
        Task<Entry> CheckOutDocumentAsync(CheckOutDocumentParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Checks in a document, creating a new version. By default releases the persistent lock.</summary>
        Task<Entry> CheckInDocumentAsync(CheckInDocumentParameters parameters, CancellationToken cancellationToken = default);

        /// <summary>Undoes a document check-out without creating a new version.</summary>
        Task<Entry> UndoCheckOutAsync(UndoCheckOutParameters parameters, CancellationToken cancellationToken = default);
    }

    #endregion

    #region Implementation

    partial class EntriesClient
    {
        private string BuildDocumentUrl(string repositoryId, int entryId, string suffix)
        {
            var sb = new StringBuilder();
            sb.Append("v2/Repositories/");
            sb.Append(Uri.EscapeDataString(repositoryId));
            sb.Append("/Entries/");
            sb.Append(Uri.EscapeDataString(entryId.ToString(CultureInfo.InvariantCulture)));
            sb.Append(suffix);
            return sb.ToString();
        }

        private async Task<T> PostJsonAsync<T>(string url, object body, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage())
            {
                if (body != null)
                {
                    var json = JsonConvert.SerializeObject(body, JsonSerializerSettings);
                    request.Content = new StringContent(json);
                }
                else
                {
                    request.Content = new StringContent(string.Empty);
                }
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var urlBuilder = new StringBuilder(url);
                PrepareRequest(_httpClient, request, urlBuilder);
                request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
                PrepareRequest(_httpClient, request, urlBuilder.ToString());

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                    if (response.Content?.Headers != null)
                        foreach (var item in response.Content.Headers) headers[item.Key] = item.Value;
                    ProcessResponse(_httpClient, response);
                    var status = (int)response.StatusCode;
                    if (status == 200)
                    {
                        var obj = await ReadObjectResponseAsync<T>(response, headers, cancellationToken).ConfigureAwait(false);
                        if (obj.Object == null) throw ApiExceptionExtensions.Create(status, headers, null);
                        return obj.Object;
                    }
                    else
                    {
                        var problem = await ReadObjectResponseAsync<ProblemDetails>(response, headers, cancellationToken).ConfigureAwait(false);
                        if (problem.Object != null) throw ApiExceptionExtensions.Create(status, headers, problem.Object, null);
                        var data = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw ApiExceptionExtensions.Create(status, headers, data, JsonSerializerSettings, null);
                    }
                }
                finally { response.Dispose(); }
            }
        }

        private async Task<T> GetJsonAsync<T>(string url, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var urlBuilder = new StringBuilder(url);
                PrepareRequest(_httpClient, request, urlBuilder);
                request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
                PrepareRequest(_httpClient, request, urlBuilder.ToString());

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                    if (response.Content?.Headers != null)
                        foreach (var item in response.Content.Headers) headers[item.Key] = item.Value;
                    ProcessResponse(_httpClient, response);
                    var status = (int)response.StatusCode;
                    if (status == 200)
                    {
                        var obj = await ReadObjectResponseAsync<T>(response, headers, cancellationToken).ConfigureAwait(false);
                        if (obj.Object == null) throw ApiExceptionExtensions.Create(status, headers, null);
                        return obj.Object;
                    }
                    else
                    {
                        var problem = await ReadObjectResponseAsync<ProblemDetails>(response, headers, cancellationToken).ConfigureAwait(false);
                        if (problem.Object != null) throw ApiExceptionExtensions.Create(status, headers, problem.Object, null);
                        var data = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw ApiExceptionExtensions.Create(status, headers, data, JsonSerializerSettings, null);
                    }
                }
                finally { response.Dispose(); }
            }
        }

        private async Task DeleteAsync(string url, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("DELETE");

                var urlBuilder = new StringBuilder(url);
                PrepareRequest(_httpClient, request, urlBuilder);
                request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
                PrepareRequest(_httpClient, request, urlBuilder.ToString());

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                    if (response.Content?.Headers != null)
                        foreach (var item in response.Content.Headers) headers[item.Key] = item.Value;
                    ProcessResponse(_httpClient, response);
                    var status = (int)response.StatusCode;
                    if (status == 204 || status == 200) return;
                    else
                    {
                        var problem = await ReadObjectResponseAsync<ProblemDetails>(response, headers, cancellationToken).ConfigureAwait(false);
                        if (problem.Object != null) throw ApiExceptionExtensions.Create(status, headers, problem.Object, null);
                        var data = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw ApiExceptionExtensions.Create(status, headers, data, JsonSerializerSettings, null);
                    }
                }
                finally { response.Dispose(); }
            }
        }

        public async Task<LockInfo> LockDocumentAsync(LockDocumentParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/Lock");
            return await PostJsonAsync<LockInfo>(url, parameters.Request, cancellationToken);
        }

        public async Task<LockInfo> GetDocumentLockInfoAsync(GetDocumentLockInfoParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/Lock");
            return await GetJsonAsync<LockInfo>(url, cancellationToken);
        }

        public async Task UnlockDocumentAsync(UnlockDocumentParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/Lock");
            if (parameters.LockToken != null)
                url += "?lockToken=" + Uri.EscapeDataString(parameters.LockToken);
            await DeleteAsync(url, cancellationToken);
        }

        public async Task<Entry> PutUnderVersionControlAsync(PutUnderVersionControlParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/VersionControl");
            return await PostJsonAsync<Entry>(url, null, cancellationToken);
        }

        public async Task<Entry> CheckOutDocumentAsync(CheckOutDocumentParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/CheckOut");
            return await PostJsonAsync<Entry>(url, parameters.Request, cancellationToken);
        }

        public async Task<Entry> CheckInDocumentAsync(CheckInDocumentParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/CheckIn");
            return await PostJsonAsync<Entry>(url, parameters.Request, cancellationToken);
        }

        public async Task<Entry> UndoCheckOutAsync(UndoCheckOutParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (parameters.RepositoryId == null) throw new ArgumentNullException("parameters.RepositoryId");
            var url = BuildDocumentUrl(parameters.RepositoryId, parameters.EntryId, "/Document/UndoCheckOut");
            return await PostJsonAsync<Entry>(url, null, cancellationToken);
        }
    }

    #endregion

    #region Parameter Classes

    public partial class LockDocumentParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
        public LockDocumentRequest Request { get; set; }
    }

    public partial class GetDocumentLockInfoParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
    }

    public partial class UnlockDocumentParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
        /// <summary>Optional lock token for administrative unlock of another user's lock.</summary>
        public string LockToken { get; set; }
    }

    public partial class PutUnderVersionControlParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
    }

    public partial class CheckOutDocumentParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
        public CheckOutDocumentRequest Request { get; set; }
    }

    public partial class CheckInDocumentParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
        public CheckInDocumentRequest Request { get; set; }
    }

    public partial class UndoCheckOutParameters
    {
        public string RepositoryId { get; set; }
        public int EntryId { get; set; }
    }

    #endregion

    #region DTOs

    public partial class LockInfo
    {
        [JsonProperty("lockToken", NullValueHandling = NullValueHandling.Ignore)]
        public string LockToken { get; set; }

        [JsonProperty("owner", NullValueHandling = NullValueHandling.Ignore)]
        public string Owner { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("extent", NullValueHandling = NullValueHandling.Ignore)]
        public string Extent { get; set; }

        [JsonProperty("creationTimestampUtc", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreationTimestampUtc { get; set; }

        [JsonProperty("entryId")]
        public int EntryId { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public partial class LockDocumentRequest
    {
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("extent", NullValueHandling = NullValueHandling.Ignore)]
        public string Extent { get; set; }
    }

    public partial class CheckOutDocumentRequest
    {
        [JsonProperty("lock")]
        public bool Lock { get; set; } = true;

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }
    }

    public partial class CheckInDocumentRequest
    {
        [JsonProperty("unlock")]
        public bool Unlock { get; set; } = true;
    }

    #endregion
}
