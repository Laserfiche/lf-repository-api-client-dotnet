// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributeValueByKeyTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAttributeByKey_ReturnAttribute()
        {
            var result = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId).ConfigureAwait(false);
            var attributeKeys = result.Value;
            Assert.IsNotNull(attributeKeys);
            Assert.IsTrue(attributeKeys.Count > 0, "No attribute keys exist on the user.");

            var attribute = await client.AttributesClient.GetTrusteeAttributeValueByKeyAsync(RepositoryId, attributeKeys.First().Key).ConfigureAwait(false);
            Assert.IsTrue(!string.IsNullOrEmpty(attribute.Value));
        }
    }
}
