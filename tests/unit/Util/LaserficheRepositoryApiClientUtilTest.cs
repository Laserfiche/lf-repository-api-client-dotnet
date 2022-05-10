using Laserfiche.Repository.Api.Client.Util;
using System;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Util
{
    public class LaserficheRepositoryApiClientUtilTest
    {
        [Theory]
        [InlineData("https://api.laserfiche.com/repository/v1/Repositories/r-123abc456/Entries/1234567/links")]
        [InlineData("https://api.laserfiche.com/repository/v1/Repositories/r-123abc456/Entries/1234567/links?$select=linkId")]
        [InlineData("https://api.laserfiche.com/repository/v1/Repositories('r-123abc456')/Entries(1234567)")]
        public void ExtractRepositoryIdFromUri_Success(string uriString)
        {
            var uri = new Uri(uriString);
            var repositoryId = RepositoryApiClientUtil.ExtractRepositoryIdFromUri(uri);
            Assert.Equal("r-123abc456", repositoryId);
        }

        [Fact]
        public void ExtractRepositoryIdFromUri_Fail()
        {
            var uri = new Uri("https://api.laserfiche.com/repository/v1/Repositories/");
            var repositoryId = RepositoryApiClientUtil.ExtractRepositoryIdFromUri(uri);
            Assert.Equal(string.Empty, repositoryId);
        }

        [Fact]
        public void GetRepositoryBaseUri_Success()
        {
            string domain = "laserfiche.com";
            var uri = RepositoryApiClientUtil.GetRepositoryBaseUri(domain);
            Assert.Equal($"https://api.{domain}/repository/", uri);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetRepositoryBaseUri_DefaultUri(string domain)
        {
            var uri = RepositoryApiClientUtil.GetRepositoryBaseUri(domain);
            Assert.Equal($"https://api.laserfiche.com/repository/", uri);
        }
    }
}
