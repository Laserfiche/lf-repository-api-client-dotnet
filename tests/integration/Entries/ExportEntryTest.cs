using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ExportEntryTest : BaseTest
    {
        int createdEntryId;
        string fileToWriteTo;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
            fileToWriteTo = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(fileToWriteTo))
            {
                File.Delete(fileToWriteTo);
            }
            if (createdEntryId != 0)
            {
                await DeleteEntry(createdEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ExportEntry()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ExportEntry";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var result = await client.EntriesClient.ExportEntryAsync(new ExportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new ExportEntryRequest()
                {
                    Part = ExportEntryRequestPart.Image,
                    ImageOptions = new ExportEntryRequestImageOptions() { Format = ExportEntryRequestImageFormat.PDF },
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result?.Value);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(result.Value).ConfigureAwait(false);

            Assert.AreEqual(entryName + ".pdf", response.Content.Headers.ContentDisposition.FileNameStar);
            Assert.AreEqual("application/pdf", response.Content.Headers.ContentType.ToString());
            Assert.IsTrue(response.Content.Headers.ContentLength > 0);

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            fileToWriteTo = Path.GetTempFileName();
            using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
            await responseStream.CopyToAsync(streamToWriteTo).ConfigureAwait(false);
        }
    }
}
