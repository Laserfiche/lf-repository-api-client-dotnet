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

        [TestMethod]
        public async Task GetEntryListing_Paging()
        {
            int entryId = 1;
            int maxMageSize = 10;
            
            bool PagingCallback(ODataValueContextOfIListOfODataEntry data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxMageSize);
                    return true; // If data aren't exhusted, keep asking.
                }
                else
                {
                    return false;
                }
            }

            await client.GetEntryListingForEachAsync(PagingCallback, TestConfig.RepositoryId, entryId, null, null, null, string.Format("maxpagesize={0}", maxMageSize));
        }
    }
}
