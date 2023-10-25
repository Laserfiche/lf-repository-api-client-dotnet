// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        CLOUD_ACCESS_KEY,
        API_SERVER_USERNAME_PASSWORD
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
        protected string BaseUrl;

        private const string TestHeaderVar = "TEST_HEADER";
        private const string AccessKeyVar = "ACCESS_KEY";
        private const string SpKeyVar = "SERVICE_PRINCIPAL_KEY";
        private const string RepoKeyVar = "REPOSITORY_ID";
        private const string UsernameVar = "APISERVER_USERNAME";
        private const string PasswordVar = "APISERVER_PASSWORD";
        private const string BaseUrlVar = "APISERVER_REPOSITORY_API_BASE_URL";
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
            string accessKeyString = Environment.GetEnvironmentVariable(AccessKeyVar);
            if (!string.IsNullOrEmpty(accessKeyString))
                AccessKey = AccessKey.CreateFromBase64EncodedAccessKey(accessKeyString);
            RepositoryId = Environment.GetEnvironmentVariable(RepoKeyVar);
            AuthorizationType = Enum.Parse<AuthorizationType>(Environment.GetEnvironmentVariable(AuthTypeVar), ignoreCase: true);
            Username = Environment.GetEnvironmentVariable(UsernameVar);
            Password = Environment.GetEnvironmentVariable(PasswordVar);
            BaseUrl = Environment.GetEnvironmentVariable(BaseUrlVar);
        }

        public IRepositoryApiClient CreateClient()
        {
            if (client == null)
            {
                if (AuthorizationType == AuthorizationType.CLOUD_ACCESS_KEY)
                {
                    if (string.IsNullOrEmpty(ServicePrincipalKey) || AccessKey == null)
                        return null;
                    client = RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey);
                }
                else if (AuthorizationType == AuthorizationType.API_SERVER_USERNAME_PASSWORD)
                {
                    if (string.IsNullOrEmpty(RepositoryId) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(BaseUrl))
                        return null;
                    client = RepositoryApiClient.CreateFromUsernamePassword(RepositoryId, Username, Password, BaseUrl);
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
            var newEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: autoRename).ConfigureAwait(false);
            Assert.IsNotNull(newEntry);
            Assert.AreEqual(parentEntryId, newEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, newEntry.EntryType);
            return newEntry;
        }

        public async Task DeleteEntry(IRepositoryApiClient client, int entryId, DeleteEntryWithAuditReason auditReason = null)
        {
            var operation = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entryId, auditReason).ConfigureAwait(false);
            Assert.IsNotNull(operation.Token);
            var progress = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, operation.Token).ConfigureAwait(false);
            Assert.IsTrue(progress.Status == OperationStatus.InProgress || progress.Status == OperationStatus.Completed);
        }

        protected async Task<int> CreateDocument(string name)
        {
            int parentEntryId = 1;
            string fileLocation = TempPath + "test.pdf";
            var request = new PostEntryWithEdocMetadataRequest();
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var electronicDocument = new FileParameter(fileStream, "test", "application/pdf");
                var result = await client.EntriesClient.ImportDocumentAsync(RepositoryId, parentEntryId, name, autoRename: true, electronicDocument: electronicDocument, request: request).ConfigureAwait(false);

                var operations = result.Operations;
                Assert.IsNotNull(operations?.EntryCreate);
                Assert.AreEqual(0, operations.EntryCreate.Exceptions.Count);
                Assert.AreNotEqual(0, operations.EntryCreate.EntryId);
                Assert.AreEqual(0, operations.SetEdoc.Exceptions.Count);
                Assert.IsFalse(string.IsNullOrEmpty(result.DocumentLink));
                return operations.EntryCreate.EntryId;
            }
        }
    }
}
