using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Laserfiche.Repository.Api.Client
{
    internal class BaseClient
    {
        protected async Task<T> GetNextLinkAsync<T>(HttpClient httpClient, string nextLink, string prefer, Func<HttpRequestMessage, HttpClient, bool[], CancellationToken, Task<T>> sendAndProcessResponseAsync, CancellationToken cancellationToken) where T : new()
        {
            if (nextLink == null)
            {
                return default;
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

                    var response = await sendAndProcessResponseAsync(request, httpClient, new bool[] { false }, default);
                    return response;
                }
            }
        }

        protected string MergeMaxSizeIntoPrefer(int? maxSize, string prefer)
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

        // Copied from auto generated code
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }

            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null && 
                        CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) is System.Runtime.Serialization.EnumMemberAttribute attribute)
                    {
                        return attribute.Value ?? name;
                    }

                    var converted = Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted ?? string.Empty;
                }
            }
            else if (value is bool boolean)
            {
                return Convert.ToString(boolean, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[] v)
            {
                return Convert.ToBase64String(v);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((System.Array)value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }

            var result = Convert.ToString(value, cultureInfo);
            return result ?? "";
        }
    }
}
