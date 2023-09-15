﻿using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class CancelTasksTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task TaskCompletedBeforeCancellation()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net CancelOperation").ConfigureAwait(false);
            StartDeleteEntryRequest body = new();
            var taskResponse = await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            
            AssertIsNotNullOrEmpty(taskResponse.TaskId);

            await Task.Delay(5000).ConfigureAwait(false);
            var cancelTaskResponse = await client.TasksClient.CancelTasksAsync(RepositoryId, new List<string> { taskResponse.TaskId }).ConfigureAwait(false);

            Assert.IsNotNull(cancelTaskResponse);
            Assert.IsNotNull(cancelTaskResponse.Value);
            Assert.IsTrue(cancelTaskResponse.Value.Count > 0);
            Assert.AreEqual(true, cancelTaskResponse.Value.First(r => r.Id == taskResponse.TaskId).Result);
        }
    }
}
