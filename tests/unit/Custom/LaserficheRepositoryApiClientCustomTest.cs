// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class LaserficheRepositoryApiClientCustomTest
    {
        #region GetEntryAsync with url
        [Fact]
        public async Task GetEntryAsync_InvalidEntryId()
        {
            string uriString = "http://host.laserfiche.com/wrongversion/Repositories/repo123/Entries/abc?$select=1";

            var client = new RepositoryApiClient(null);

            await Assert.ThrowsAsync<ArgumentException>(() => client.EntriesClient.GetEntryAsync(uriString));
        }

        [Fact]
        public async Task GetEntryAsync_InvalidUri()
        {
            string uriString = "http://host.laserfiche.com/wrongversion/Repositories/repo123/Entries/456?$select=1";

            var client = new RepositoryApiClient(null);

            await Assert.ThrowsAsync<ArgumentException>(() => client.EntriesClient.GetEntryAsync(uriString));
        }
        #endregion
    }
}
