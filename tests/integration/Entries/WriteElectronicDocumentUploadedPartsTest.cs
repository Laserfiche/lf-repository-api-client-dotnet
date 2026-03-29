// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class WriteElectronicDocumentUploadedPartsTest : BaseTest
    {
        int createdEntryId;
        private static readonly HttpClient _HttpClient = new HttpClient();

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                await DeleteEntry(createdEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task WriteElectronicDocumentUploadedParts_Success()
        {
            // Create empty document
            var entryName = "RepositoryApiClientIntegrationTest .Net WriteEdocUploadedParts";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            Assert.IsFalse(((Document)createdEntry).IsElectronicDocument);

            // Generate a test file larger than 5MB (minimum S3 chunk size)
            // Using a 6MB byte array with PDF-like header
            int fileSizeBytes = 6 * 1024 * 1024;
            var fileBytes = new byte[fileSizeBytes];
            // PDF header
            fileBytes[0] = 0x25; fileBytes[1] = 0x50; fileBytes[2] = 0x44; fileBytes[3] = 0x46;

            // Get upload URLs for 2 parts
            var createUrlsResult = await client.EntriesClient.CreateMultipartUploadUrlsAsync(new CreateMultipartUploadUrlsParameters()
            {
                RepositoryId = RepositoryId,
                Request = new CreateMultipartUploadUrlsRequest()
                {
                    StartingPartNumber = 1,
                    NumberOfParts = 2,
                    FileName = "test.pdf",
                    MimeType = "application/pdf"
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(createUrlsResult.UploadId);
            Assert.AreEqual(2, createUrlsResult.Urls.Length);

            // Upload chunks to S3
            int chunkSize = 5 * 1024 * 1024; // 5MB
            var eTags = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                int offset = i * chunkSize;
                int length = Math.Min(chunkSize, fileSizeBytes - offset);
                var chunk = new byte[length];
                Array.Copy(fileBytes, offset, chunk, 0, length);

                var response = await _HttpClient.PutAsync(createUrlsResult.Urls[i], new ByteArrayContent(chunk)).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                if (response.Headers.TryGetValues("ETag", out var values))
                {
                    eTags.Add(values.First());
                }
            }

            // Call WriteEdocUploadedParts
            var taskResponse = await client.EntriesClient.WriteElectronicDocumentUploadedPartsAsync(new WriteElectronicDocumentUploadedPartsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new WriteEdocUploadedPartsRequest()
                {
                    UploadId = createUrlsResult.UploadId,
                    PartETags = eTags.ToArray()
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(taskResponse.TaskId);

            // Poll task until completed
            bool inProgress = true;
            TaskProgress taskProgress = null;
            int attempts = 0;

            while (inProgress && attempts < 10)
            {
                attempts++;
                await Task.Delay(5000).ConfigureAwait(false);
                var tasksList = await client.TasksClient.ListTasksAsync(new ListTasksParameters()
                {
                    RepositoryId = RepositoryId,
                    TaskIds = new[] { taskResponse.TaskId }
                }).ConfigureAwait(false);
                taskProgress = tasksList.Value.FirstOrDefault(t => t.Id.Equals(taskResponse.TaskId));
                Assert.IsNotNull(taskProgress);
                inProgress = taskProgress.Status == TaskStatus.InProgress;
            }

            Assert.AreEqual(TaskStatus.Completed, taskProgress.Status);

            // Verify edoc was written
            var doc = (Document)(await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false));

            Assert.IsTrue(doc.IsElectronicDocument);
            Assert.IsTrue(doc.ElectronicDocumentSize > 0);
        }
    }
}
