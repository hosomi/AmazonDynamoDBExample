using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AmazonDynamoDBExample.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Tests
{
    [TestClass]
    public class TestMusicRepositoryDBClient
    {
        [TestMethod]
        public async Task TestMusicRepositoryDBClientAsync()
        {
            IAmazonDynamoDB DDBClient = new AmazonDynamoDBClient("dummy", "dummy",
                new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000",
                    UseHttp = true,
                }
            );

            MusicRepository repo = new MusicRepository(DDBClient);

            string hashKey = "No One You Know";
            string rangeKey = "My Doc Spot";

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>
            {
                { "Artist", new AttributeValue { S = hashKey } },
                { "SongTitle", new AttributeValue { S = rangeKey } },
                { "AlbumTitle", new AttributeValue { S = "Hey Now" } },
                { "CriticRating", new AttributeValue { N = "8.4" } },
                { "Genre", new AttributeValue { S = "Country" } },
                { "Year", new AttributeValue { N = "1984" } }
            };

            var putItemResponse = await repo.PutItemAsync(item);
            Assert.IsNotNull(putItemResponse);
            Assert.AreEqual(putItemResponse.HttpStatusCode, HttpStatusCode.OK);


            GetItemResponse getItemResponse = await repo.GetItemAsync(hashKey, rangeKey);
            Assert.IsNotNull(getItemResponse);
            Assert.AreEqual(getItemResponse.HttpStatusCode, HttpStatusCode.OK);

            Assert.IsTrue(getItemResponse.Item.TryGetValue("Artist", out AttributeValue valueArtist));
            Assert.IsNotNull(valueArtist.S);
            Assert.AreEqual(hashKey, valueArtist.S);

            Assert.IsTrue(getItemResponse.Item.TryGetValue("SongTitle", out AttributeValue valueSongTitle));
            Assert.IsNotNull(valueSongTitle.S);
            Assert.AreEqual(rangeKey, valueSongTitle.S);

            DeleteItemResponse deleteItemResponse = await repo.DeleteItemAsync(hashKey, rangeKey);
            Assert.IsNotNull(deleteItemResponse);
            Assert.AreEqual(deleteItemResponse.HttpStatusCode, HttpStatusCode.OK);


            GetItemResponse getItemResponse2 = await repo.GetItemAsync(hashKey, rangeKey);
            Assert.IsNotNull(getItemResponse2);
            Assert.AreEqual(getItemResponse2.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getItemResponse2.Item.Count, 0);
        }
    }
}
