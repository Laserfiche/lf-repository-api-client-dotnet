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
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body);
            }
        }

        [TestMethod]
        public async Task SetTags_ReturnTags()
        {
            var tagDefinitionsResult = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId);
            var tagDefinitions = tagDefinitionsResult.Value;
            Assert.IsNotNull(tagDefinitions);
            Assert.IsTrue(tagDefinitions.Count > 0, "No tag definitions exist in the repository.");
            string tag = tagDefinitions.First().Name;
            var request = new PutTagRequest()
            {
                Tags = new List<string>() { tag }
            };
            entry = await CreateEntry(client, "APIServerClientIntegrationTest SetTags");

            var result = await client.EntriesClient.AssignTagsAsync(RepositoryId, entry.Id, request);
            var tags = result.Value;
            Assert.IsNotNull(tags);
            Assert.AreEqual(request.Tags.Count, tags.Count);
            Assert.AreEqual(tag, tags.FirstOrDefault()?.Name);
        }
    }
}
