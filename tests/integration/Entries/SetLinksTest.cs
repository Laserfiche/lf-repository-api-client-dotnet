using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetLinksTest : BaseTest
    {
        IList<Entry> createdEntries;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntries = new List<Entry>();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (var entry in createdEntries)
            {
                if (entry != null)
                {
                    StartDeleteEntryRequest body = new();
                    await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task SetAndReturnLinks()
        {
            var sourceEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetLinks Source").ConfigureAwait(false);
            createdEntries.Add(sourceEntry);
            
            var targetEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetLinks Target").ConfigureAwait(false);
            createdEntries.Add(targetEntry);
            
            var request = new SetLinksRequest()
            {
                Links = new List<LinkToUpdate>
                {
                    new LinkToUpdate
                    {
                        LinkDefinitionId = targetEntry.Id,
                    }
                }
            };

            var result = await client.EntriesClient.SetLinksAsync(RepositoryId, sourceEntry.Id, request).ConfigureAwait(false);
            var links = result.Value;

            Assert.IsNotNull(links);
            Assert.AreEqual(request.Links.Count, links.Count);
            Assert.AreEqual(sourceEntry.Id, links.FirstOrDefault()?.SourceId);
            Assert.AreEqual(targetEntry.Id, links.FirstOrDefault()?.TargetId);
        }
    }
}
