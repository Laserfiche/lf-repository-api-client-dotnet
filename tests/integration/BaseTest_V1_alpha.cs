using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class BaseTest_V1_Alpha : BaseTest
    {
        public async Task Login(ILaserficheRepositoryApiClient client, string username = null, string password = null)
        {
            string repoId = TestConfig.RepositoryId;
            var request = new CreateConnectionRequest()
            {
                Username = username ?? TestConfig.Username,
                Password = password ?? TestConfig.Password
            };
            bool? createCookie = null;
            string customerId = TestConfig.AccountId;

            var sessionKeyInfo = await client?.CreateAccessTokenAsync(repoId, request, createCookie, customerId);
            client.AccessToken = sessionKeyInfo?.Result?.AuthToken;
        }

        public async Task BeforeSendAsync(HttpRequestMessage request, ILaserficheRepositoryApiClient repositoryClient,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(repositoryClient.AccessToken) && !request.RequestUri.AbsolutePath.EndsWith("/AccessTokens/Create", StringComparison.OrdinalIgnoreCase))
            {
                await Login(repositoryClient);
            }
        }

        public Task<ILaserficheRepositoryApiClient> CreateClientAndLogin()
        {
            var repositoryApiClientOptions = new ClientOptions()
            {
                Domain = TestConfig.Domain,
                BeforeSendAsync = BeforeSendAsync
            };
            var repositoryClient = LaserficheRepositoryApiClientFactory.CreateClient(repositoryApiClientOptions);
            return Task.FromResult(repositoryClient);
        }

        public async Task Logout(ILaserficheRepositoryApiClient client)
        {
            string repoId = TestConfig.RepositoryId;
            await client?.InvalidateAccessTokenAsync(repoId);
            client.AccessToken = null;
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
