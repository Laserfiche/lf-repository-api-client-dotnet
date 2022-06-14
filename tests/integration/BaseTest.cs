using Laserfiche.Api.Client.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class BaseTest
    {
        private const string TestConfigFile = "TestConfig.env";
        protected static readonly string TempPath = @"TestFiles/";
        protected string AuthorizationType;
        protected AccessKey AccessKey;
        protected string ServicePrincipalKey;
        protected string RepositoryId;
        protected string Username;
        protected string Password;
        protected string Organization;
        protected string BaseUri;

        private const string AccessKeyVar = "ACCESS_KEY";
        private const string SpKeyVar = "SERVICE_PRINCIPAL_KEY";
        private const string RepoKeyVar = "REPOSITORY_ID";
        private const string UsernameVar = "LFDS_TEST_USERNAME";
        private const string PasswordVar = "LFDS_TEST_PASSWORD";
        private const string BaseUrlVar = "SELFHOSTED_REPOSITORY_API_BASE_URI";
        private const string OrganizationVar = "LFDS_TEST_ORGANIZATION";
        private const string AuthTypeVar = "AUTHORIZATION_TYPE";

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
                System.Diagnostics.Trace.WriteLine($"{fileName} not found.");
        }

        private static string DecodeBase64(string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable(SpKeyVar);

            var accessKeyStr = DecodeBase64(Environment.GetEnvironmentVariable(AccessKeyVar));
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(accessKeyStr);

            RepositoryId = Environment.GetEnvironmentVariable(RepoKeyVar);
            AuthorizationType = Environment.GetEnvironmentVariable(AuthTypeVar);
            Username = Environment.GetEnvironmentVariable(UsernameVar);
            Password = Environment.GetEnvironmentVariable(PasswordVar);
            Organization = Environment.GetEnvironmentVariable(OrganizationVar);
            BaseUri = Environment.GetEnvironmentVariable(BaseUrlVar);
        }

        public IRepositoryApiClient CreateClient()
        {
            if (AuthorizationType.Equals("AccessKey", StringComparison.InvariantCultureIgnoreCase))
            {
                return (string.IsNullOrEmpty(ServicePrincipalKey) || AccessKey == null) ? null : RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey);
            }
            else if (AuthorizationType.Equals("LfdsUsernamePassword", StringComparison.InvariantCultureIgnoreCase))
            {
                return (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Organization) || string.IsNullOrEmpty(BaseUri)) ? null : RepositoryApiClient.CreateFromLfdsUsernamePassword(Username, Password, Organization, RepositoryId, BaseUri);
            }
            return null;
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
