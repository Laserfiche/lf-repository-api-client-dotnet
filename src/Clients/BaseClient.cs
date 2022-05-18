using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client
{
    internal class BaseClient
    {
        protected async Task<SwaggerResponse<T>> GetNextLinkAsync<T>(HttpClient httpClient, string nextLink, string prefer, Func<HttpRequestMessage, HttpClient, bool[], CancellationToken, Task<SwaggerResponse<T>>> sendAndProcessResponseAsync, CancellationToken cancellationToken) where T : new()
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

            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }

                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool)
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[])value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }

            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
}
