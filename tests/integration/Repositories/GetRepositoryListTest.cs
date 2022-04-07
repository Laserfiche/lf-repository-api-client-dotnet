using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Repositories
{
    [TestClass]
    public class GetRepositoryListTest : BaseTest_V1
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
        public async Task GetRepositoryList_ReturnSuccessful()
        {
            var response = await client.GetRepositoryListAsync();
            Assert.IsTrue(response.Result.Count > 0, "No repositories found.");

            bool foundRepo = false;
            foreach (var repoInfo in response.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(repoInfo.RepoId));
                Assert.IsFalse(string.IsNullOrEmpty(repoInfo.WebclientUrl));
                Assert.IsTrue(repoInfo.WebclientUrl.Contains(repoInfo.RepoId));

                if (repoInfo.RepoId == TestConfig.RepositoryId)
                    foundRepo = true;
            }
            Assert.IsTrue(foundRepo);
        }
    }
}
