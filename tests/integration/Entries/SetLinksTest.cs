using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetLinksTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        IList<Entry> createdEntries;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
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
                    await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, entry.Id, body);
                    Thread.Sleep(5000);
                }
            }
            await Logout(client);
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

            var response = await client.AssignEntryLinksAsync(TestConfig.RepositoryId, sourceEntry.Id, request);

            var links = response.Result?.Value;
            Assert.IsNotNull(links);
            Assert.AreEqual(request.Count, links.Count);
            Assert.AreEqual(sourceEntry.Id, links.FirstOrDefault()?.SourceId);
            Assert.AreEqual(targetEntry.Id, links.FirstOrDefault()?.TargetId);
        }
    }
}
