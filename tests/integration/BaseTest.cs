using Laserfiche.Oauth.Api.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class BaseTest
    {
        protected static readonly string TempPath = @"TestFiles\";
        private static TestConfig _testConfig;
        public static TestConfig TestConfig {
            get {
                if (_testConfig == null)
                    _testConfig = CreateTestConfig();
                return _testConfig;
            }
        }

        protected IClientCredentialsHandler oauthClient;

        public BaseTest()
        { 
        }

        public static TestConfig CreateTestConfig()
        {
            string testingConfig = "";
            testingConfig = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"TestConfig.json"));
            if (string.IsNullOrEmpty(testingConfig))
            {
                throw new Exception("Cannot load TestConfig.json");
            }
            var config = JsonConvert.DeserializeObject<TestConfig>(testingConfig);
            return config;
        }

        public async Task<IClientCredentialsHandler> GetOauthClient()
        {
            if (oauthClient == null)
            {
                var configPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "ServiceAppConfig.json");
                oauthClient = await ClientCredentialsHandler.CreateFromAccessKeyAsync(configPath);
            }
            return oauthClient;
        }
    }
}
