﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetTagsTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        Entry entry;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
            entry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (entry != null)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, entry.Id, body);
                Thread.Sleep(5000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task SetTags_ReturnTags()
        {
            var tagDefinitionsResponse = await client.GetTagDefinitionsAsync(TestConfig.RepositoryId);
            var tagDefinitions = tagDefinitionsResponse.Result?.Value;
            Assert.IsNotNull(tagDefinitions);
            Assert.IsTrue(tagDefinitions.Count > 0, "No tag definitions exist in the repository.");
            string tag = tagDefinitions.First().Name;
            var request = new PutTagRequest()
            {
                Tags = new List<string>() { tag }
            };
            entry = await CreateEntry(client, "APIServerClientIntegrationTest SetTags");

            var response = await client.AssignTagsAsync(TestConfig.RepositoryId, entry.Id, request);
            var tags = response.Result?.Value;
            Assert.IsNotNull(tags);
            Assert.AreEqual(request.Tags.Count, tags.Count);
            Assert.AreEqual(tag, tags.FirstOrDefault()?.Name);
        }
    }
}
