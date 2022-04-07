using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CreateCopyEntryTest : BaseTest_V1
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
        public async Task CreateCopyEntry_CreateFolder()
        {
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Folder,
                Name = newEntryName
            };

            var response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var entry = response.Result;
            Assert.IsNotNull(entry);
            createdEntries.Add(entry);
            Assert.AreEqual(parentEntryId, entry.ParentId);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
            Assert.AreEqual(typeof(Folder), entry.GetType());
        }

        [TestMethod]
        public async Task CreateCopyEntry_CreateShortcut()
        {
            // Create new entry
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Folder,
                Name = newEntryName
            };
            var response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var targetEntry = response.Result;
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Create shortcut to the new entry
            newEntryName = "APIServerClientIntegrationTest CreateShortcut";
            request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Shortcut,
                Name = newEntryName,
                TargetId = targetEntry.Id
            };
            response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var shortcut = response.Result;
            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);
            Assert.AreEqual(typeof(Shortcut), shortcut.GetType());
            Assert.AreEqual(targetEntry.EntryType, ((Shortcut)shortcut).TargetType);
            Assert.AreEqual(targetEntry.Id, ((Shortcut)shortcut).TargetId);
        }


        [TestMethod]
        public async Task CreateCopyEntry_CopyShortcut()
        {
            // Create new entry
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Folder,
                Name = newEntryName
            };
            var response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var targetEntry = response.Result;
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);
            
            // Create shortcut to the new entry
            newEntryName = "APIServerClientIntegrationTest CreateShortcut";
            request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Shortcut,
                Name = newEntryName,
                TargetId = targetEntry.Id
            };
            response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var shortcut = response.Result;

            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);

            // Copy entry
            request = new PostEntryChildrenRequest()
            {
                Name = "CopiedEntry",
                SourceId = shortcut.Id
            };
            response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            
            string redirectUrl = "";
            redirectUrl = response.Headers["Location"].ToList()[0];

            response = await client.GetEntryAsync(redirectUrl);
            createdEntries.Add(response.Result);
            Assert.IsTrue(response.Result.Name.StartsWith(request.Name));
            Assert.AreEqual(parentEntryId, response.Result.ParentId);
            Assert.AreEqual(shortcut.EntryType, response.Result.EntryType);
        }


        [ExpectedException(typeof(ApiException<ProblemDetails>))]
        [TestMethod]
        public async Task CreateCopyEntry_CopyFolder()
        {
            // Create new entry
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = EntryType.Folder,
                Name = newEntryName
            };
            var response = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
            var targetEntry = response.Result;
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            request = new PostEntryChildrenRequest()
            {
                Name = "CopiedEntry",
                SourceId = targetEntry.Id
            };
            _ = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: true);
        }
    }
}
