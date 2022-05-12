using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CopyEntryAsyncTest : BaseTest
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
                    Thread.Sleep(5000);
                }
            }
        }

        [TestMethod]
        public async Task CreateCopyEntry_CopyEntry()
        {
            // Create new entry
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var response = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true);
            var targetEntry = response.Result;
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            var copyrequest = new CopyAsyncRequest()
            {
                Name = "CopiedEntry",
                SourceId = targetEntry.Id
            };
            string opToken = "";
            var copyresponse = await client.EntriesClient.CopyEntryAsync(RepositoryId, parentEntryId, copyrequest, autoRename: true);
            opToken = copyresponse.Result.Token;

            await Task.Delay(5000);
            string redirectUrl = "";
            var opResponse = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, opToken);
            redirectUrl = opResponse.Headers["Location"].ToList()[0];

            response = await client.EntriesClient.GetEntryAsync(redirectUrl);
            createdEntries.Add(response.Result);
            Assert.IsTrue(response.Result.Name.StartsWith(copyrequest.Name));
            Assert.AreEqual(parentEntryId, response.Result.ParentId);
            Assert.AreEqual(targetEntry.EntryType, response.Result.EntryType);
        }
    }
}
