using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laserfiche.Repository.Api.Client
{
    partial class PostEntryWithEdocMetadataRequest : IFormattable
    {
        /// <inheritdoc/>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider is System.Globalization.CultureInfo cultureInfo)
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Culture = cultureInfo });
            }
            return JsonConvert.SerializeObject(this);
        }
    }

    partial class CreateEntryResult
    {
        /// <summary>
        /// Returns a human-readable summary of the <see cref="CreateEntryResult"/>.
        /// </summary>
        /// <returns>A human-readable summary of the <see cref="CreateEntryResult"/>.</returns>
        public string GetSummary()
        {
            var messages = new List<string>();
            int entryId = Operations?.EntryCreate?.EntryId ?? default;
            if (entryId != default)
            {
                messages.Add($"{nameof(Operations.EntryCreate.EntryId)}={entryId}.");
            }
            messages.Add(GetErrorMessages(Operations?.EntryCreate?.Exceptions));
            messages.Add(GetErrorMessages(Operations?.SetEdoc?.Exceptions));
            messages.Add(GetErrorMessages(Operations?.SetTemplate?.Exceptions));
            messages.Add(GetErrorMessages(Operations?.SetFields?.Exceptions));
            messages.Add(GetErrorMessages(Operations?.SetTags?.Exceptions));
            messages.Add(GetErrorMessages(Operations?.SetLinks?.Exceptions));
            return string.Join(" ", messages.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        private string GetErrorMessages(ICollection<APIServerException> errors)
        {
            if (errors == null)
                return string.Empty;
            return string.Join(" ", errors?.Select(e => e.Message));
        }
    }

    #region inheritance
    [JsonConverter(typeof(JsonInheritanceConverter), "entryType")]
    [JsonInheritance("Document", typeof(Document))]
    [JsonInheritance("Folder", typeof(Folder))]
    [JsonInheritance("Shortcut", typeof(Shortcut))]
    [JsonInheritance("RecordSeries", typeof(RecordSeries))]
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
