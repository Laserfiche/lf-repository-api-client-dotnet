﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class GetOperationStatusTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetOperationStatus_ReturnStatus()
        {
            var deleteEntry = await CreateEntry(client, "APIServerClientIntegrationTest GetOperationStatus");

            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var response = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body);
            var token = response.Result?.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            var operationProgressResponse = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, token);
            var operationProgress = operationProgressResponse.Result;
            Assert.IsNotNull(operationProgress);
            Assert.AreEqual(OperationStatus.Completed, operationProgress.Status);
            Assert.AreEqual(100, operationProgress.PercentComplete);
        }
    }
}
