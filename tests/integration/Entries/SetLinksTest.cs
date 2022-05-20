using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetLinksTest : BaseTest
    {
        IRepositoryApiClient client = null;
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
                    DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                    await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body);
                }
            }
        }

        [TestMethod]
        public async Task SetLinks_ReturnLinks()
        {
            var sourceEntry = await CreateEntry(client, "APIServerClientIntegrationTest SetLinks Source");
            createdEntries.Add(sourceEntry);
            var targetEntry = await CreateEntry(client, "APIServerClientIntegrationTest SetLinks Target");
            createdEntries.Add(targetEntry);
            var request = new List<PutLinksRequest>()
            {
                new PutLinksRequest()
                {
                    TargetId = targetEntry.Id,
                    LinkTypeId = 1
                }
            };

            var result = await client.EntriesClient.AssignEntryLinksAsync(RepositoryId, sourceEntry.Id, request);

            var links = result.Value;
            Assert.IsNotNull(links);
            Assert.AreEqual(request.Count, links.Count);
            Assert.AreEqual(sourceEntry.Id, links.FirstOrDefault()?.SourceId);
            Assert.AreEqual(targetEntry.Id, links.FirstOrDefault()?.TargetId);
        }
    }
}
