using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmazonDynamoDBExample.Tests
{
    [TestClass]
    public class TestMultipleCols : IDisposable
    {
        string TableName { get; }
        IAmazonDynamoDB DDBClient { get; }

        public TestMultipleCols()
        {
            this.TableName = "MultipleCols-" + DateTime.Now.Ticks;

            //this.DDBClient = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
            // test is DnaymoDB local.
            this.DDBClient = new AmazonDynamoDBClient("dummy", "dummy",
                new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000",
                    UseHttp = true,
                }
            );

            SetupTableAsync().Wait();
        }

        [TestMethod]
        public async Task TestColsAddAsync()
        {

            string id = Guid.NewGuid().ToString();
            Console.WriteLine(id);

            Dictionary<string, AttributeValue> keyItem = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            };

            PutItemResponse putResponse = await DDBClient.PutItemAsync(TableName, keyItem);
            Assert.AreEqual(putResponse.HttpStatusCode, HttpStatusCode.OK);


            Dictionary<string, AttributeValueUpdate> updateItem = new Dictionary<string, AttributeValueUpdate>
            {
                { "Col1",  new AttributeValueUpdate { Action =  AttributeAction.PUT, Value = new AttributeValue { N = "0" } } }
            };

            UpdateItemResponse updateResponse = await DDBClient.UpdateItemAsync(TableName, keyItem, updateItem);
            Assert.AreEqual(updateResponse.HttpStatusCode, HttpStatusCode.OK);

            Dictionary<string, AttributeValueUpdate> updateItem2 = new Dictionary<string, AttributeValueUpdate>
            {
                { "Col1",  new AttributeValueUpdate { Action =  AttributeAction.PUT, Value = new AttributeValue { N = "1" } } }
            };

            UpdateItemResponse updateResponse2 =  await DDBClient.UpdateItemAsync(TableName, keyItem, updateItem2);
            Assert.AreEqual(updateResponse2.HttpStatusCode, HttpStatusCode.OK);


            GetItemResponse getItemResponse = await DDBClient.GetItemAsync(TableName, keyItem);
            Assert.AreEqual(getItemResponse.HttpStatusCode, HttpStatusCode.OK);
            bool isId = getItemResponse.Item.TryGetValue("Id", out AttributeValue id2);
            Assert.IsTrue(isId);
            Assert.AreEqual(id, id2.S);

            bool isCol1 = getItemResponse.Item.TryGetValue("Col1", out AttributeValue co1);
            Assert.IsTrue(isCol1);
            Assert.AreEqual("1", co1.N);
        }

        [TestMethod]
        public async Task TestColsLargeAmountsAddAsync()
        {
            string id = Guid.NewGuid().ToString();
            Console.WriteLine(id);

            Dictionary<string, AttributeValue> keyItem = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            };

            PutItemResponse putResponse = await DDBClient.PutItemAsync(TableName, keyItem);
            Assert.AreEqual(putResponse.HttpStatusCode, HttpStatusCode.OK);

            int colSize = 500;
            Dictionary<string, AttributeValueUpdate> updateItem = new Dictionary<string, AttributeValueUpdate>
            {
                { "ColSize",  new AttributeValueUpdate { Action =  AttributeAction.PUT, Value = new AttributeValue { N = $"{colSize}" } } }
            };

            Enumerable.Range(1, colSize).ToList().ForEach(i =>
            {

                updateItem.Add($"Col{i}", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = "0" } });
            });

            UpdateItemResponse updateResponse = await DDBClient.UpdateItemAsync(TableName, keyItem, updateItem);
            Assert.AreEqual(updateResponse.HttpStatusCode, HttpStatusCode.OK);
        }


        private async Task SetupTableAsync()
        {

            CreateTableRequest request = new CreateTableRequest
            {
                TableName = this.TableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        KeyType = KeyType.HASH,
                        AttributeName = "Id"
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = ScalarAttributeType.S
                    }
                }
            };

            await this.DDBClient.CreateTableAsync(request);

            var describeRequest = new DescribeTableRequest { TableName = this.TableName };
            DescribeTableResponse response = null;
            do
            {
                Thread.Sleep(1000);
                response = await this.DDBClient.DescribeTableAsync(describeRequest);
            } while (response.Table.TableStatus != TableStatus.ACTIVE);
        }


        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.DDBClient.DeleteTableAsync(this.TableName).Wait();
                    this.DDBClient.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

    }
}
