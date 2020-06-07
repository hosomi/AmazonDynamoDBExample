using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Tests
{
    [TestClass]
    public class TestMusicDBClient
    {
        [TestMethod]
        public async Task TestMusicDBClientAsync()
        {
            IAmazonDynamoDB DDBClient = new AmazonDynamoDBClient("dummy", "dummy",
                new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000",
                    UseHttp = true,
                }
            );

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>
            {
                { "Artist", new AttributeValue { S = "No One You Know" } },
                { "SongTitle", new AttributeValue { S = "My Doc Spot" } },
                { "AlbumTitle", new AttributeValue { S = "Hey Now" } },
                { "CriticRating", new AttributeValue { N = "8.4" } },
                { "Genre", new AttributeValue { S = "Country" } },
                { "Year", new AttributeValue { N = "1984" } }
            };

            await DDBClient.PutItemAsync("Music", item);

            var request = new GetItemRequest
            {
                TableName = "Music",
                Key = new Dictionary<string, AttributeValue>() {
                    {"Artist", new AttributeValue { S = "No One You Know" } },
                    { "SongTitle", new AttributeValue { S = "My Doc Spot" } }
                }
            };

            GetItemResponse response = await DDBClient.GetItemAsync(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);

            Assert.IsTrue(response.Item.TryGetValue("Artist", out AttributeValue valueArtist));
            Assert.IsNotNull(valueArtist.S);
            Assert.AreEqual("No One You Know", valueArtist.S);

            Assert.IsTrue(response.Item.TryGetValue("SongTitle", out AttributeValue valueSongTitle));
            Assert.IsNotNull(valueSongTitle.S);
            Assert.AreEqual("My Doc Spot", valueSongTitle.S);

            var requestDelete = new DeleteItemRequest
            {
                TableName = "Music",
                Key = new Dictionary<string, AttributeValue>() {
                    {"Artist", new AttributeValue { S = "No One You Know" } },
                    { "SongTitle", new AttributeValue { S = "My Doc Spot" } }
                }
            };
            DeleteItemResponse responseDelete = await DDBClient.DeleteItemAsync(requestDelete);
            Assert.IsNotNull(responseDelete);
            Assert.AreEqual(responseDelete.HttpStatusCode, HttpStatusCode.OK);

            var request2 = new GetItemRequest
            {
                TableName = "Music",
                Key = new Dictionary<string, AttributeValue>() {
                    {"Artist", new AttributeValue { S = "No One You Know" } },
                    { "SongTitle", new AttributeValue { S = "My Doc Spot" } }
                }
            };
            GetItemResponse response2 = await DDBClient.GetItemAsync(request2);
            Assert.IsNotNull(response2);
            Assert.AreEqual(response2.Item.Count, 0);
        }
    }
}
