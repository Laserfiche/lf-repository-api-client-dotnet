// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var result = await client.AttributesClient.ListAttributesAsync(new ListAttributesParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
            var attributeKeys = result.Value;
            Assert.IsNotNull(attributeKeys);
            Assert.IsTrue(attributeKeys.Count > 0, "No attribute keys exist on the user.");

            var attribute = await client.AttributesClient.GetAttributeAsync(new GetAttributeParameters()
            {
                RepositoryId = RepositoryId,
                AttributeKey = attributeKeys.First().Key
            }).ConfigureAwait(false);
            AssertIsNotNullOrEmpty(attribute.Value);
        }
    }
}
