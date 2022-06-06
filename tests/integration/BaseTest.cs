using Laserfiche.Api.Client.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class BaseTest
    {
        private const string TestConfigFile = "TestConfig.env";
        protected static readonly string TempPath = @"TestFiles/";
        protected AccessKey AccessKey;
        protected string ServicePrincipalKey;
        protected string RepositoryId;

        public BaseTest()
        {
            TryLoadFromDotEnv(TestConfigFile);
            PopulateFromEnv();
        }

        private static void TryLoadFromDotEnv(string fileName)
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName);
            if (File.Exists(path))
            {
                DotNetEnv.Env.Load(path, new DotNetEnv.LoadOptions(
                    setEnvVars: true,
                    clobberExistingVars: true,
                    onlyExactPath: true
                ));
                System.Diagnostics.Trace.TraceWarning($"{fileName} found. {fileName} file should only be used in local developer computers.");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"{fileName} not found.");
            }
        }

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable("DEV_CA_PUBLIC_USE_TESTOAUTHSERVICEPRINCIPAL_SERVICE_PRINCIPAL_KEY");
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(Environment.GetEnvironmentVariable("DEV_CA_PUBLIC_USE_INTEGRATION_TEST_ACCESS_KEY"));
            RepositoryId = Environment.GetEnvironmentVariable("DEV_CA_PUBLIC_USE_REPOSITORY_ID_1");
        }

        public IRepositoryApiClient CreateClient()
        {
            return RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey);
        }

        public async Task<Entry> CreateEntry(IRepositoryApiClient client, string entryName, int parentEntryId = 1, bool autoRename = true)
        {
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = entryName
            };
            var newEntry = await client?.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: autoRename);
            Assert.IsNotNull(newEntry);
            Assert.AreEqual(parentEntryId, newEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, newEntry.EntryType);
            return newEntry;
        }
    }
}
