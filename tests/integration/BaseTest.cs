using Laserfiche.Api.Client.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public enum AuthorizationType
    {
        AccessKey,
        LfdsUsernamePassword
    }

    public class BaseTest
    {
        private const string TestConfigFile = ".env";
        protected static readonly string TempPath = @"TestFiles/";
        protected AuthorizationType AuthorizationType;
        protected string TestHeader;
        protected AccessKey AccessKey;
        protected string ServicePrincipalKey;
        protected string RepositoryId;
        protected string Username;
        protected string Password;
        protected string Organization;
        protected string BaseUri;

        private const string TestHeaderVar = "TEST_HEADER";
        private const string AccessKeyVar = "ACCESS_KEY";
        private const string SpKeyVar = "SERVICE_PRINCIPAL_KEY";
        private const string RepoKeyVar = "REPOSITORY_ID";
        private const string UsernameVar = "LFDS_USERNAME";
        private const string PasswordVar = "LFDS_PASSWORD";
        private const string BaseUrlVar = "SELFHOSTED_REPOSITORY_API_BASE_URI";
        private const string OrganizationVar = "LFDS_ORGANIZATION";
        private const string AuthTypeVar = "AUTHORIZATION_TYPE";

        private const string ApplicationNameHeaderKey = "X-LF-AppID";
        private const string ApplicationNameHeaderValue = "RepositoryApiClientIntegrationTest .Net";
        public static IRepositoryApiClient client = null;

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

        private void PopulateFromEnv()
        {
            TestHeader = Environment.GetEnvironmentVariable(TestHeaderVar);
            ServicePrincipalKey = Environment.GetEnvironmentVariable(SpKeyVar);
            AccessKey = AccessKey.CreateFromBase64EncodedAccessKey(Environment.GetEnvironmentVariable(AccessKeyVar));
            RepositoryId = Environment.GetEnvironmentVariable(RepoKeyVar);
            AuthorizationType = Enum.Parse<AuthorizationType>(Environment.GetEnvironmentVariable(AuthTypeVar), ignoreCase: true);
            Username = Environment.GetEnvironmentVariable(UsernameVar);
            Password = Environment.GetEnvironmentVariable(PasswordVar);
            Organization = Environment.GetEnvironmentVariable(OrganizationVar);
            BaseUri = Environment.GetEnvironmentVariable(BaseUrlVar);
        }

        public IRepositoryApiClient CreateClient()
        {
            if (client == null)
            {
                if (AuthorizationType == AuthorizationType.AccessKey)
                {
                    if (string.IsNullOrEmpty(ServicePrincipalKey) || AccessKey == null)
                        return null;
                    client = RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey);
                }
                else if (AuthorizationType == AuthorizationType.LfdsUsernamePassword)
                {
                    if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Organization) || string.IsNullOrEmpty(BaseUri))
                        return null;
                    client = RepositoryApiClient.CreateFromLfdsUsernamePassword(Username, Password, Organization, RepositoryId, BaseUri);
                }

                client.DefaultRequestHeaders.Add(ApplicationNameHeaderKey, ApplicationNameHeaderValue);
                if (!string.IsNullOrEmpty(TestHeader))
                {
                    client.DefaultRequestHeaders.Add(TestHeader, "true");
                }
            }
            return client;
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
