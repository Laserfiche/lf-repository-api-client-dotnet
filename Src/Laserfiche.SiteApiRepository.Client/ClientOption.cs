using System;
using System.Collections.Generic;
using System.Text;

namespace Laserfiche.SiteApiRepository.Client
{
    public class ClientOptions
    {
        public static string DefaultBaseUrl { get; private set; } = "https://api.laserfiche.com/repository";
        public string BaseUrl { get; set; } = DefaultBaseUrl;
        public bool AllowAutoRedirect { get; set; } = false;
    }
}
