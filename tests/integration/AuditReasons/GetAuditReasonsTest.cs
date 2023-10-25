// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AuditReasons
{
    [TestClass]
    public class GetAuditReasonsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAuditReasons_ReturnAuditReasons()
        {
            var auditReasons = await client.AuditReasonsClient.GetAuditReasonsAsync(RepositoryId).ConfigureAwait(false);

            Assert.IsNotNull(auditReasons);
            Assert.IsNotNull(auditReasons.DeleteEntry);
            Assert.IsNotNull(auditReasons.ExportDocument);
        }
    }
}
