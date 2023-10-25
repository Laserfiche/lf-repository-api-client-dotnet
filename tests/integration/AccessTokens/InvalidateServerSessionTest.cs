// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class InvalidateServerSessionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task InvalidateServerSession_LogoutSuccessful()
        {
            if (AuthorizationType == AuthorizationType.API_SERVER_USERNAME_PASSWORD)
            {
                return;
            }

            var result = await client.ServerSessionClient.InvalidateServerSessionAsync(RepositoryId).ConfigureAwait(false);
            Assert.AreEqual(true, result.Value);
        }
    }
}
