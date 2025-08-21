// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class GetOperationStatusTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetOperationStatus_ReturnStatus()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net GetOperationStatus").ConfigureAwait(false);

            Client.AuditReasons auditReasons = await client.AuditReasonsClient.GetAuditReasonsAsync(RepositoryId);
            Client.WAuditReason deleteReason = auditReasons.DeleteEntry.FirstOrDefault();
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            if (deleteReason != null)
            {
                body.AuditReasonId = deleteReason.Id;
                body.Comment = "test comment";
            }
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            var token = result.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            await Task.Delay(5000).ConfigureAwait(false);

            var operationProgress = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, token).ConfigureAwait(false);
            Assert.IsNotNull(operationProgress);
            Assert.AreEqual(OperationStatus.Completed, operationProgress.Status);
            Assert.AreEqual(100, operationProgress.PercentComplete);
        }
    }
}
