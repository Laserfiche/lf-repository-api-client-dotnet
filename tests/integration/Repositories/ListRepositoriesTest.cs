using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Repositories
{
    [TestClass]
    public class ListRepositoriesTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ListRepositories_ReturnSuccessful()
        {
            var repositoryCollectionResponse = await client.RepositoriesClient.ListRepositoriesAsync().ConfigureAwait(false);
            AssertCollectionResponse(repositoryCollectionResponse);

            bool repositoryFound = false;
            foreach (var repository in repositoryCollectionResponse.Value)
            {
                Assert.IsFalse(string.IsNullOrEmpty(repository.Id));
                
                if (AuthorizationType != AuthorizationType.API_SERVER_USERNAME_PASSWORD)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(repository.WebClientUrl));
                    Assert.IsTrue(repository.WebClientUrl.Contains(repository.Id));
                }

                if (repository.Id.Equals(RepositoryId, System.StringComparison.OrdinalIgnoreCase))
                    repositoryFound = true;
            }
            
            Assert.IsTrue(repositoryFound);
        }

        [TestMethod]
        public async Task GetSelfHostedRepositoryList_ReturnSuccessful()
        {
            if (AuthorizationType == AuthorizationType.CLOUD_ACCESS_KEY)
            {
                return; // There's no point testing if it is a cloud environment
            }
            var repositoryCollectionResponse = await RepositoriesClient.GetSelfHostedRepositoryListAsync(BaseUrl).ConfigureAwait(false);
            AssertCollectionResponse(repositoryCollectionResponse);
            
            bool repositoryFound = false;
            foreach (var repository in repositoryCollectionResponse.Value)
            {
                Assert.IsFalse(string.IsNullOrEmpty(repository.Id));

                if (repository.Id.Equals(RepositoryId, System.StringComparison.OrdinalIgnoreCase))
                    repositoryFound = true;
            }

            Assert.IsTrue(repositoryFound);
        }
    }
}
