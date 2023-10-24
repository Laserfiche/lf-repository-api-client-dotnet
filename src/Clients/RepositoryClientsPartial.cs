// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Laserfiche.Repository.Api.Client
{
    /// <summary>
    /// Represents a file to be uploaded.
    /// </summary>
    public partial class FileParameter
    {
        /// <summary>
        /// Constructor for representing a file to be uploaded.
        /// </summary>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file to be uploaded. The file extension in the name will be used as the file extension of the imported entry. For common file extensions, the mime type may be derived from the file extension.</param>
        public FileParameter(Stream data, string fileName) : this(data, fileName, null)
        {
        }

        /// <summary>
        /// Constructor for representing a file to be uploaded.
        /// </summary>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file to be uploaded. The file extension in the name will be used as the extension of the imported entry.</param>
        /// <param name="mimeType">The mime-type of the file to be uploaded.</param>
        public FileParameter(Stream data, string fileName, string mimeType)
        {
            Data = data;
            FileName = fileName;
            MimeType = mimeType;
        }

        /// <summary>
        /// The file data.
        /// </summary>
        public Stream Data { get; private set; }

        /// <summary>
        /// The name of the file to be uploaded. The file extension in the name will be used as the extension of the imported entry.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The mime-type of the file to be uploaded.
        /// </summary>
        public string MimeType { get; private set; }

        internal string ContentType
        {
            get { return MimeType; }
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
    [GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v11.0.0.0)")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class JsonInheritanceAttribute : System.Attribute
    {
        public JsonInheritanceAttribute(string key, Type type)
        {
            Key = key;
            Type = type;
        }

        public string Key { get; }

        public Type Type { get; }
    }

    [GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v11.0.0.0)")]
    internal class JsonInheritanceConverter : JsonConverter
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                _isWriting = true;

                var jObject = JObject.FromObject(value, serializer);
                try
                {
                    jObject.AddFirst(new JProperty(_discriminator, GetSubtypeDiscriminator(value.GetType())));
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

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<JObject>(reader);
            if (jObject == null)
                return null;

            var discriminatorValue = jObject.GetValue(_discriminator);
            var discriminator = discriminatorValue != null ? Extensions.Value<string>(discriminatorValue) : null;
            var subtype = GetObjectSubtype(objectType, discriminator);

            var objectContract = serializer.ContractResolver.ResolveContract(subtype) as JsonObjectContract;
            if (objectContract == null || Enumerable.All(objectContract.Properties, p => p.PropertyName != _discriminator))
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

        private Type GetObjectSubtype(Type objectType, string discriminator)
        {
            foreach (var attribute in CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Key == discriminator)
                    return attribute.Type;
            }

            return objectType;
        }

        private string GetSubtypeDiscriminator(Type objectType)
        {
            foreach (var attribute in CustomAttributeExtensions.GetCustomAttributes<JsonInheritanceAttribute>(IntrospectionExtensions.GetTypeInfo(objectType), true))
            {
                if (attribute.Type == objectType)
                    return attribute.Key;
            }

            return objectType.Name;
        }
    }
    #endregion
}
