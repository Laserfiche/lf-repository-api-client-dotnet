using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryFieldsTest : BaseTest_V1
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
        public async Task GetEntryFields_ReturnFields()
        {
            int entryId = 1;
            var response = await client.GetFieldValuesAsync(TestConfig.RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetEntryFields_Paging()
        {
            int entryId = 1;
            int maxMageSize = 10;

            bool PagingCallback(ODataValueContextOfIListOfFieldValue data)
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

            await client.GetFieldValuesForEachAsync(PagingCallback, TestConfig.RepositoryId, entryId, string.Format("maxpagesize={0}", maxMageSize));
        }
    }
}
