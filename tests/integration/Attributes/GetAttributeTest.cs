﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributeTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnAttribute()
        {
            var result = await client.AttributesClient.ListAttributesAsync(RepositoryId).ConfigureAwait(false);
            var attributeKeys = result.Value;
            Assert.IsNotNull(attributeKeys);
            Assert.IsTrue(attributeKeys.Count > 0, "No attribute keys exist on the user.");

            var attribute = await client.AttributesClient.GetAttributeAsync(RepositoryId, attributeKeys.First().Key).ConfigureAwait(false);
            AssertIsNotNullOrEmpty(attribute.Value);
        }
    }
}
