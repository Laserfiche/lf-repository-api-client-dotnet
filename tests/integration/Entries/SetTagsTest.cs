// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetTagsTest : BaseTest
    {
        Entry entry;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            entry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (entry != null)
            {
                await DeleteEntry(entry.Id).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task SetAndReturnTags()
        {
            var tagDefinitionsResult = await client.TagDefinitionsClient.ListTagDefinitionsAsync(new ListTagDefinitionsParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
            var tagDefinitions = tagDefinitionsResult.Value;
            
            Assert.IsNotNull(tagDefinitions);
            Assert.IsTrue(tagDefinitions.Count > 0, "No tag definitions exist in the repository.");

            var informationalTag = tagDefinitions.FirstOrDefault(t => t.IsSecure == false && !t.Name.Contains("Automatically select tags"));
            if (informationalTag == null)
            {
                Assert.Inconclusive(
                    $"No informational (IsSecure=false) tag definitions were returned by the server for repository '{RepositoryId}'. " +
                    $"All {tagDefinitions.Count} tag(s) have IsSecure=true or the server omitted the isSecure field. " +
                    "Add at least one informational tag definition to the repository to enable this test.");
            }
            string tag = informationalTag.Name;
            var request = new SetTagsRequest()
            {
                Tags = new List<string>() { tag }
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetTags").ConfigureAwait(false);

            var setResult = await client.EntriesClient.SetTagsAsync(new SetTagsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entry.Id,
                Request = request
            }).ConfigureAwait(false);
            var setTags = setResult.Value;

            // Independently verify the tag was actually applied by listing the entry's tags.
            // This guards against the PUT response being empty even when the tag was set, and
            // provides a clearer failure message if the service principal cannot apply the tag.
            var listResult = await client.EntriesClient.ListTagsAsync(new ListTagsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entry.Id
            }).ConfigureAwait(false);
            var listedTags = listResult.Value;

            Assert.IsNotNull(listedTags,
                $"ListTagsAsync returned null after SetTagsAsync with tag '{tag}'.");
            Assert.AreEqual(1, listedTags.Count,
                $"Expected 1 tag on the entry after SetTagsAsync, but ListTagsAsync returned {listedTags.Count}. " +
                $"Tag used: '{tag}' (IsSecure={informationalTag.IsSecure}). " +
                "This may mean the service principal cannot apply this tag despite it being informational.");
            Assert.AreEqual(tag, listedTags.FirstOrDefault()?.Name,
                $"Tag name mismatch after SetTagsAsync. Expected '{tag}'.");

            // Also assert on the SetTagsAsync PUT response itself.
            Assert.IsNotNull(setTags,
                $"SetTagsAsync returned null Value in the response for tag '{tag}'.");
            Assert.AreEqual(1, setTags.Count,
                $"SetTagsAsync PUT response contained {setTags.Count} tag(s) instead of 1. " +
                "The tag WAS applied (verified via ListTagsAsync), but the PUT response is incorrect.");
            Assert.AreEqual(tag, setTags.FirstOrDefault()?.Name,
                $"SetTagsAsync PUT response tag name mismatch. Expected '{tag}'.");
        }
    }
}
