using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class BaseTest_V1 : BaseTest
    {
        public async Task<ILaserficheRepositoryApiClient> CreateClientAndLogin()
        {
            var oauthClient = await GetOauthClient();
            var repositoryApiClientOptions = ClientOptionsFactory.CreateClientOptions(oauthClient.Configuration.Domain, oauthClient.GetAccessTokenAsync, oauthClient.RefreshAccessTokenAsync);
            return LaserficheRepositoryApiClientFactory.CreateClient(repositoryApiClientOptions);
        }

        public async Task Logout(ILaserficheRepositoryApiClient client)
        {
            string repoId = TestConfig.RepositoryId;
            await client?.InvalidateServerSessionAsync(repoId);
        }

        public async Task<Entry> CreateEntry(ILaserficheRepositoryApiClient client, string entryName, int parentEntryId = 1, bool autoRename = true)
        {
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = entryName
            };
            var newEntry = await client.CreateOrCopyEntryAsync(TestConfig.RepositoryId, parentEntryId, request, autoRename: autoRename);
            Assert.IsNotNull(newEntry.Result);
            Assert.AreEqual(parentEntryId, newEntry.Result.ParentId);
            Assert.AreEqual(EntryType.Folder, newEntry.Result.EntryType);
            return newEntry.Result;
        }
    }
}
