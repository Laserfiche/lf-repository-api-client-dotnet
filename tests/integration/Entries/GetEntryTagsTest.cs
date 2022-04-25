using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryTagsTest : BaseTest_V1
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
        public async Task GetEntryTags_ReturnTags()
        {
            int entryId = 1;
            var response = await client.GetTagsAssignedToEntryAsync(TestConfig.RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetEntryTags_Paging()
        {
            int entryId = 1;
            int maxMageSize = 10;

            bool PagingCallback(ODataValueContextOfIListOfWTagInfo data)
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

            await client.GetTagsAssignedToEntryForEachAsync(PagingCallback, TestConfig.RepositoryId, entryId, string.Format("maxpagesize={0}", maxMageSize));
        }
    }
}
