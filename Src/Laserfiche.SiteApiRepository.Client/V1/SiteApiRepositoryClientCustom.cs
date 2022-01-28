using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Laserfiche.SiteApiRepository.Client.Test")]
namespace Laserfiche.SiteApiRepository.Client.V1
{
    partial interface ISiteApiRepositoryClient
    {
        void SetAuthToken(string accessToken);
        string GetAuthToken();
        void RemoveAuthToken();
        System.Threading.Tasks.Task<SwaggerResponse<Entry>> Get_entry_infoAsync(string uriString, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }

    partial class SiteApiRepositoryClient : ISiteApiRepositoryClient
    {
        public void SetAuthToken(string accessToken)
        {
            var authentication = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Authorization = authentication;
        }

        public string GetAuthToken()
        {
            string authToken = _httpClient.DefaultRequestHeaders?.Authorization?.Parameter;
            return authToken;
        }

        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// Get entry with redirect url. If url validation fail, it will throw exception.
        /// </summary>
        /// <param name="uriString">Redirect url string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SwaggerResponse<Entry>> Get_entry_infoAsync(string uriString, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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
            return await Get_EntryAsync(repoId, entryId, select, cancellationToken);
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
                catch (Exception e) { }
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
