// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.OAuth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
                    client = RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey, "repository.ReadWrite");
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
            var request = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = entryName,
                AutoRename = autoRename
            };
            var newEntry = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(newEntry);
            Assert.AreEqual(parentEntryId, newEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, newEntry.EntryType);
            
            return newEntry;
        }

        public async Task DeleteEntry(int entryId, StartDeleteEntryRequest request = null)
        {
            var operation = await client.EntriesClient.StartDeleteEntryAsync(new StartDeleteEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId,
                Request = request
            }).ConfigureAwait(false);
            Assert.IsNotNull(operation.TaskId);
        }

        protected async Task<Entry> CreateDocument(string name)
        {
            int parentEntryId = 1;
            string fileLocation = TempPath + "test.pdf";
            var request = new ImportEntryRequest()
            {
                AutoRename = true,
                Name = name,
                PdfOptions = new ImportEntryRequestPdfOptions()
                {
                    GeneratePages = true,
                }
            };

            using var fileStream = File.OpenRead(fileLocation);
            var electronicDocument = new FileParameter(fileStream, "test.pdf", "application/pdf");
            var entry = await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                File = electronicDocument,
                Request = request
            }).ConfigureAwait(false);

            Assert.IsNotNull(entry);
            Assert.IsNotNull(entry.Id);

            return entry;
        }

        protected static void AssertCollectionResponse(AttributeCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(AuditReasonCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(FieldDefinitionCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(LinkDefinitionCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(EntryCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(FieldCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(TagCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(TagDefinitionCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(LinkCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(RepositoryCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(SearchContextHitCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(TaskCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertCollectionResponse(TemplateDefinitionCollectionResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsTrue(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
        }

        protected static void AssertIsNotNullOrEmpty(string value)
        {
            Assert.IsFalse(value.IsNullOrEmpty());
        }
    }
}
