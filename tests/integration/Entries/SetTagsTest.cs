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
            
            string tag = tagDefinitions.First().Name;
            var request = new SetTagsRequest()
            {
                Tags = new List<string>() { tag }
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetTags").ConfigureAwait(false);

            var result = await client.EntriesClient.SetTagsAsync(new SetTagsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entry.Id,
                Request = request
            }).ConfigureAwait(false);
            var tags = result.Value;
            
            Assert.IsNotNull(tags);
            Assert.AreEqual(request.Tags.Count, tags.Count);
            Assert.AreEqual(tag, tags.FirstOrDefault()?.Name);
        }
    }
}
