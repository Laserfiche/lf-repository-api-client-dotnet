// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetDocumentContentTypeTest : BaseTest
    {
        int createdEntryId;

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
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, createdEntryId, body).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task GetDocumentContentTypeAsync_ReturnHeaders()
        {
            createdEntryId = await CreateDocument("RepositoryApiClientIntegrationTest .Net GetDocumentContentTypeAsync").ConfigureAwait(false);

            var response = await client.EntriesClient.GetDocumentContentTypeAsync(RepositoryId, createdEntryId).ConfigureAwait(false);

            Assert.AreEqual(200, response.StatusCode);
            Assert.IsTrue(response.Headers.ContainsKey("Content-Type"));
            Assert.IsTrue(response.Headers.ContainsKey("Content-Length"));
        }

        [TestMethod]
        public async Task GetDocumentContentTypeAsync_ThrowException()
        {
            int entryId = 1;
            string repositoryId = "fakeRepository";
            try
            {
                await client.EntriesClient.GetDocumentContentTypeAsync(repositoryId, entryId).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.IsNotNull(e?.ProblemDetails?.Title);
                Assert.AreEqual(e.ProblemDetails.Title, e.Message);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.StatusCode);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.ProblemDetails.Status);
                Assert.IsNotNull(e.ProblemDetails.OperationId);
                Assert.IsNull(e.ProblemDetails.Type);
                Assert.IsNull(e.ProblemDetails.Instance);
                Assert.IsNull(e.ProblemDetails.ErrorSource);
                Assert.AreEqual(default, e.ProblemDetails.ErrorCode);
                Assert.IsNull(e.ProblemDetails.TraceId);
                Assert.AreEqual(0, e.ProblemDetails.Extensions.Count);
            }
        }
    }
}
