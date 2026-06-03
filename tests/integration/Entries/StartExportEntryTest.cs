// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class StartExportEntryTest : BaseTest
    {
        int createdEntryId;
        string fileToWriteTo;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
            fileToWriteTo = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(fileToWriteTo))
            {
                File.Delete(fileToWriteTo);
            }
            if (createdEntryId != 0)
            {
                await DeleteEntry(createdEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task StartExportEntry()
        {
            // Unique per run: the integration tests share a cloud repo, so a fixed name
            // collides with leftovers from a previous run (or a concurrent run) — LFS then
            // auto-renames the new entry (e.g. "... StartExportEntry (3)") and the exported
            // filename no longer matches the asserted name. A GUID keeps each run isolated.
            var entryName = $"RepositoryApiClientIntegrationTest .Net StartExportEntry {System.Guid.NewGuid()}";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var request = new StartExportEntryRequest()
            {
                Part = ExportEntryRequestPart.Image,
                ImageOptions = new ExportEntryRequestImageOptions() { Format = ExportEntryRequestImageFormat.PDF },
            };

            // If needed, add export audit reason to request body
            var auditReasonCollectionResponse = await client.AuditReasonsClient.ListAuditReasonsAsync(new ListAuditReasonsParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
            var exportAuditReason = auditReasonCollectionResponse.Value.FirstOrDefault(ar => ar.AuditEventType == AuditEventType.ExportDocument);
            if (exportAuditReason != null)
            {
                request.AuditReasonId = exportAuditReason.Id;
            }

            // Export entry
            var result = await client.EntriesClient.StartExportEntryAsync(new StartExportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = request
            }).ConfigureAwait(false);

            Assert.IsNotNull(result?.TaskId);

            // Poll until the export long-operation reaches a terminal status instead of asserting
            // after a single fixed delay. Under shared-repo load the operation can take longer than
            // one interval, which previously flaked this assert (work item #671227).
            TaskCollectionResponse taskCollectionResponse;
            var pollDeadline = System.DateTime.UtcNow.AddSeconds(120);
            do
            {
                await Task.Delay(2000).ConfigureAwait(false);
                taskCollectionResponse = await client.TasksClient.ListTasksAsync(new ListTasksParameters()
                {
                    RepositoryId = RepositoryId,
                    TaskIds = new[] { result.TaskId }
                }).ConfigureAwait(false);
                var current = taskCollectionResponse.Value.FirstOrDefault(t => t.Id == result.TaskId);
                if (current != null && current.Status != TaskStatus.InProgress && current.Status != TaskStatus.NotStarted)
                    break;
            } while (System.DateTime.UtcNow < pollDeadline);

            AssertCollectionResponse(taskCollectionResponse);
            var task = taskCollectionResponse.Value.FirstOrDefault(t => t.Id == result.TaskId);
            Assert.IsNotNull(task);
            Assert.AreEqual(TaskStatus.Completed, task.Status);

            // Download exported entry
            string downloadLink = task.Result.Uri;
            Assert.IsNotNull(downloadLink);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(downloadLink).ConfigureAwait(false);

            // Tolerate an auto-rename suffix (e.g. " (2)") inserted before the extension when a
            // prior run's entry lingers on the shared repo: assert the unique prefix + extension
            // instead of an exact filename match (work item #671227).
            var exportedFileName = response.Content.Headers.ContentDisposition.FileNameStar;
            Assert.IsTrue(exportedFileName.StartsWith(entryName), $"Expected exported filename to start with '{entryName}' but was '{exportedFileName}'.");
            Assert.IsTrue(exportedFileName.EndsWith(".pdf"), $"Expected exported filename to end with '.pdf' but was '{exportedFileName}'.");
            Assert.AreEqual("application/pdf", response.Content.Headers.ContentType.ToString());
            Assert.IsTrue(response.Content.Headers.ContentLength > 0);

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            fileToWriteTo = Path.GetTempFileName();
            using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
            await responseStream.CopyToAsync(streamToWriteTo).ConfigureAwait(false);
        }
    }
}
