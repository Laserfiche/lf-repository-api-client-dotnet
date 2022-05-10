using System;
using System.Text.RegularExpressions;

namespace Laserfiche.Repository.Api.Client.Util
{
    public static class RepositoryApiClientUtil
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
        /// Returns the Laserfiche repository api base uri using the Laserfiche account id.
        /// </summary>
        /// <param name="accountId">The Laserfiche account id.</param>
        /// <returns>The Laserfiche repository api base uri.</returns>
        public static string GetRepositoryBaseUri(string accountId)
        {
            string host = GetRepositoryBaseUriHost(accountId);
            return $"https://{host}/repository/";
        }

        /// <summary>
        /// Returns the Laserfiche repository api base uri host using the Laserfiche account id.
        /// </summary>
        /// <param name="accountId">The Laserfiche account id.</param>
        /// <returns>The Laserfiche repository api base uri host.</returns>
        public static string GetRepositoryBaseUriHost(string accountId)
        {
            string domain = GetDomainFromAccountId(accountId);
            return $"api.{domain}";
        }

        /// <summary>
        /// Returns the Laserfiche domain using the Laserfiche account id.
        /// </summary>
        /// <param name="accountId">The Laserfiche account id.</param>
        /// <returns>The Laserfiche domain.</returns>
        public static string GetDomainFromAccountId(string accountId)
        {
            if (accountId?.Length == 10)
            {
                if (accountId.StartsWith("1"))
                {
                    return "laserfiche.ca";
                }
                else if (accountId.StartsWith("2"))
                {
                    return "eu.laserfiche.com";
                }
            }
            return "laserfiche.com";
        }
    }
}
