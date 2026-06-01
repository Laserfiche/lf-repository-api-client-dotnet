// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.OAuth;
using Laserfiche.Api.Client.Utils;
using Laserfiche.Repository.Api.Client.IntegrationTest.Util;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

        // Populated by MSTest via property injection. Used by CheckSkipIfEndpointMissing
        // to read the running test method's name for method-level
        // <see cref="SkipIfEndpointMissingAttribute"/> resolution.
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Skips the test as Inconclusive when the test class or method is decorated with
        /// <see cref="SkipIfEndpointMissingAttribute"/> and any named operationId is absent
        /// from the swagger document at <see cref="BaseUrl"/>. Method-level attributes take
        /// precedence over class-level. Replaces the
        /// <c>[Ignore("Temporarily ignored: cloud test server not yet updated...")]</c> pattern.
        /// </summary>
        [TestInitialize]
        public async Task CheckSkipIfEndpointMissing()
        {
            var attr = ResolveSkipAttribute();
            if (attr == null || attr.OperationIds.Length == 0) return;

            var probeBaseUrl = ResolveSwaggerProbeBaseUrl();
            var (ops, fetchError) = await SwaggerOperationCache.GetAsync(probeBaseUrl);
            if (fetchError != null)
            {
                SkipInconclusive($"Could not fetch swagger from {probeBaseUrl?.TrimEnd('/')}/swagger/v2/swagger.json: {fetchError.GetType().Name}: {fetchError.Message}");
            }

            var missing = attr.OperationIds.Where(id => !ops.Contains(id)).ToList();
            if (missing.Count == 0) return;

            // Production never silently skips. A missing endpoint on prod is either a
            // genuine regression or an incomplete deploy, and we want it to surface with
            // its natural failure (the client call hits the absent route and throws).
            // Deploy lag — the only reason this skip exists — only happens against
            // clouddev/cloudtest, never against prod.
            if (IsProductionEnvironment(probeBaseUrl))
            {
                ResolveTestContext()?.WriteLine(
                    $"[SkipIfEndpointMissing] Missing at {probeBaseUrl}: {string.Join(", ", missing)}. " +
                    $"Running anyway because BaseUrl appears to target a production environment.");
                return;
            }

            SkipInconclusive($"Endpoint(s) not deployed at {probeBaseUrl}: {string.Join(", ", missing)}");
        }

        // CI uses CLOUD_ACCESS_KEY auth without setting APISERVER_REPOSITORY_API_BASE_URL —
        // the V2 client resolves the regional API URL from the AccessKey's domain via
        // DomainUtils internally. Mirror that here so the swagger probe targets the same
        // server the client itself will hit. If neither is available the helper returns
        // null and the cache surfaces a clean ArgumentException-shaped Inconclusive.
        private string ResolveSwaggerProbeBaseUrl()
        {
            if (!string.IsNullOrEmpty(BaseUrl)) return BaseUrl;
            if (AccessKey != null && !string.IsNullOrEmpty(AccessKey.Domain))
                return DomainUtils.GetRepositoryApiBaseUri(AccessKey.Domain);
            return null;
        }

        // Treat anything that isn't clearly clouddev or cloudtest as production. The client
        // repo has no enum-based environment configuration like the server repo's
        // runsettings/TestEnvironment; the URL substring is the only signal we get. Unknown
        // hosts (including malformed URLs) fall to the "treat as prod" side, which fails
        // closed — a real regression won't disappear into a silent Inconclusive.
        private static bool IsProductionEnvironment(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) return false;
            return baseUrl.IndexOf("clouddev", StringComparison.OrdinalIgnoreCase) < 0
                && baseUrl.IndexOf("cloudtest", StringComparison.OrdinalIgnoreCase) < 0;
        }

        // Mirrors the Inconclusive message to the test console output before raising the
        // assert. Without this the message only lands in the test-result detail pane —
        // visible in the Azure DevOps Tests tab, but easy to miss when scanning the
        // pipeline task log where Inconclusive otherwise looks like a silent skip.
        private void SkipInconclusive(string message)
        {
            ResolveTestContext()?.WriteLine($"[SKIP] {message}");
            Assert.Inconclusive(message);
        }

        private SkipIfEndpointMissingAttribute ResolveSkipAttribute()
        {
            // Method-level wins over class-level.
            SkipIfEndpointMissingAttribute methodAttr = null;
            var testName = ResolveTestContext()?.TestName;
            if (!string.IsNullOrEmpty(testName))
            {
                // Strip parameterized-test arg suffix (e.g., "Foo (1, 2)"); GetMethod doesn't
                // resolve those names. If the strip yields an unknown name, GetMethod returns
                // null and we fall through to class-level cleanly.
                var paren = testName.IndexOf('(');
                var methodName = paren > 0 ? testName.Substring(0, paren).Trim() : testName;
                methodAttr = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                    ?.GetCustomAttribute<SkipIfEndpointMissingAttribute>(inherit: true);
            }
            return methodAttr ?? GetType().GetCustomAttribute<SkipIfEndpointMissingAttribute>(inherit: true);
        }

        // TestContext is read via reflection so a derived class that shadows the property
        // still surfaces the value MSTest set — reflection picks up the most-derived
        // declaration, which is the one MSTest populates.
        private TestContext ResolveTestContext()
        {
            var tcProp = GetType().GetProperty("TestContext", BindingFlags.Public | BindingFlags.Instance);
            return tcProp?.GetValue(this) as TestContext;
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
                    client = RepositoryApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey, "repository.ReadWrite", BaseUrl);
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

        protected async Task<Entry> CreateEmptyDocument(string name)
        {
            int parentEntryId = 1;
            var request = new ImportEntryRequest()
            {
                AutoRename = true,
                Name = name,
            };

            using var emptyStream = new MemoryStream();
            var electronicDocument = new FileParameter(emptyStream, name, "application/octet-stream");
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
