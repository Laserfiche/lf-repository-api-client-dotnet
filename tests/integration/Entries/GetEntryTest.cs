// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnRootFolder()
        {
            int entryId = 1;
            var entry = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Folder), entry.GetType());
            Assert.AreEqual(entryId, entry.Id);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
        }

        [TestMethod]
        public async Task ThrowException()
        {
            int entryId = 1;
            string fakeRepository = "fakeRepository";
            try
            {
                _ = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
                {
                    RepositoryId = fakeRepository,
                    EntryId = entryId
                }).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.IsNotNull(e?.ProblemDetails?.Title);
                Assert.AreEqual(e.ProblemDetails.Title, e.Message);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.StatusCode);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.ProblemDetails.Status);
                Assert.IsNotNull(e.ProblemDetails.OperationId);
                Assert.IsNotNull(e.ProblemDetails.Type);
                Assert.IsNotNull(e.ProblemDetails.Instance);
                Assert.IsNotNull(e.ProblemDetails.ErrorSource);
                Assert.AreNotEqual(default, e.ProblemDetails.ErrorCode);
                Assert.IsNotNull(e.ProblemDetails.TraceId);
                Assert.AreEqual(0, e.ProblemDetails.Extensions.Count);
            }
        }
    }
}
