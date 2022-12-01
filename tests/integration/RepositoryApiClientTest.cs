﻿using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    [TestClass]
    public class RepositoryApiClientTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task FailedAuthentication_ThrowsException()
        {
            int entryId = 1;
            IRepositoryApiClient invalidClient;
            switch (AuthorizationType)
            {
                case AuthorizationType.CloudAccessKey:
                    invalidClient = RepositoryApiClient.CreateFromAccessKey("a wrong service principal key", AccessKey);
                    break;
                case AuthorizationType.APIServerUsernamePassword:
                    invalidClient = RepositoryApiClient.CreateFromUsernamePassword(RepositoryId, Username, "wrong_password", BaseUrl);
                    break;
                default:
                    Assert.Fail("AuthorizationType not implemented.");
                    return;
            }

            var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await invalidClient.EntriesClient.GetEntryAsync(RepositoryId, entryId));

            Assert.AreEqual(401, exception.ProblemDetails.Status);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.IsNotNull(exception.ProblemDetails.Title);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.IsNotNull(exception.ProblemDetails.Type);
            Assert.IsNotNull(exception.ProblemDetails.OperationId);
        }
    }
}
