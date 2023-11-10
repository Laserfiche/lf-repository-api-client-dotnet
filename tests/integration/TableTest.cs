// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Laserfiche.Table.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    [TestClass]
    public class TableTest : BaseTest
    {
        ITableApiClient tableClient;
        [TestInitialize]
        public void Initialize()
        {
            tableClient = CreateTableClient();
        }

        private ITableApiClient CreateTableClient()
        {
            if (tableClient == null)
            {
                //if (AuthorizationType == AuthorizationType.CLOUD_ACCESS_KEY)
                //{
                //    if (string.IsNullOrEmpty(ServicePrincipalKey) || AccessKey == null)
                //        return null;
                    tableClient = TableApiClient.CreateFromAccessKey(ServicePrincipalKey, AccessKey, "table.ReadWrite", "https://api.a.clouddev.laserfiche.ca/table/");
                //}

                tableClient.DefaultRequestHeaders.Add(ApplicationNameHeaderKey, ApplicationNameHeaderValue);
                if (!string.IsNullOrEmpty(TestHeader))
                {
                    tableClient.DefaultRequestHeaders.Add(TestHeader, "true");
                }
            }
            return tableClient;
        }

        [TestMethod]
        public async Task TablesAsyncTest()
        {
            var result = await tableClient.TableClient.TablesAsync(new TablesParameters()
            {
            }).ConfigureAwait(false);
            var tables = result.Value;
            Assert.IsNotNull(tables);
            Assert.IsTrue(tables.Count > 0, "No table exist on the user.");

        }
    }
}
