using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Repositories
{
    [TestClass]
    public class GetRepositoryListTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetRepositoryList_ReturnSuccessful()
        {
            var result = await client.RepositoriesClient.GetRepositoryListAsync();
            Assert.IsTrue(result.Count > 0, "No repositories found.");

            bool foundRepo = false;
            foreach (var repoInfo in result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(repoInfo.RepoId));
                Assert.IsFalse(string.IsNullOrEmpty(repoInfo.WebclientUrl));
                Assert.IsTrue(repoInfo.WebclientUrl.Contains(repoInfo.RepoId));

                if (repoInfo.RepoId == RepositoryId)
                    foundRepo = true;
            }
            Assert.IsTrue(foundRepo);
        }
    }
}
