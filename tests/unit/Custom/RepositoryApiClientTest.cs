// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class RepositoryApiClientTest
    {
        [Fact]
        public void Create_ExceptionHasMinimalProblemDetails()
        {
            // Arrange
            string repositoryId = "repositoryId";
            string username = "username";
            string password = "password";
            string baseUrl = "http://example.com/";

            // Act
            var client = RepositoryApiClient.CreateFromUsernamePassword(repositoryId, username, password, baseUrl);

            // Assert
            Assert.NotNull(client);

        }


    }
}
