using System;
using System.Text.RegularExpressions;

namespace Laserfiche.Repository.Api.Client.Util
{
    public static class LaserficheRepositoryApiClientUtil
    {
        /// <summary>
        /// Returns the repository id from a Laserfiche repository api uri.
        /// </summary>
        /// <param name="uri">The Laserfiche repository api uri.</param>
        /// <returns>The repository id.</returns>
        public static string ExtractRepositoryIdFromUri(Uri uri)
        {
            var matches = Regex.Matches(uri.LocalPath, @"\/Repositories(?:\/|\(')(?<repoId>[a-zA-Z0-9\-]*?)(?:'\))?\/",
                RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            if (matches.Count > 0)
            {
                return matches[0].Groups["repoId"].Value;
            }
            return "";
        }

        /// <summary>
        /// Returns the Laserfiche repository api base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The Laserfiche repository api base uri.</returns>
        public static string GetRepositoryBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                domain = "laserfiche.com";
            return $"https://api.{domain}/repository/";
        }
    }
}
