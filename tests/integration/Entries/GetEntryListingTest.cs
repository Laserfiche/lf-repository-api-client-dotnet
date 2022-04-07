using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryListingTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await Logout(client);
        }

        [TestMethod]
        public async Task GetEntryListing_ReturnEntries()
        {
            int entryId = 1;
            var response = await client.GetEntryListingAsync(TestConfig.RepositoryId, entryId);
            var entries = response.Result?.Value;
            Assert.IsNotNull(entries);
            foreach(var entry in entries)
            {
                Assert.AreEqual(entryId, entry.ParentId);
            }
        }
    }
}
