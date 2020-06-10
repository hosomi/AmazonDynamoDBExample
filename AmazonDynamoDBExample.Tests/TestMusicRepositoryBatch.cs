using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AmazonDynamoDBExample.Entities;
using AmazonDynamoDBExample.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Tests
{
    [TestClass]
    public class TestMusicRepositoryBatch
    {
        [TestMethod]
        public async Task TestMusicRepositoryBatchAsync()
        {
            IAmazonDynamoDB DDBClient = new AmazonDynamoDBClient("dummy", "dummy",
                new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000",
                    UseHttp = true,
                }
            );

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            var DDBContext = new DynamoDBContext(DDBClient, config);

            List<Music> writeList = new List<Music>() {
                new Music
                {
                    Artist = "No One You Know",
                    SongTitle = "My Doc Spot",
                    AlbumTitle = "Hey Now",
                    CriticRating = 8.4,
                    Genre = "Country",
                    Year = 1984
                },
                new Music
                {
                    Artist = "No One You Know",
                    SongTitle = "Somewhere Down The Road",
                    AlbumTitle = "Somewhar Famous",
                    CriticRating = 8.4,
                    Genre = "Country",
                    Year = 1984
                },
                new Music
                {
                    Artist = "The Acme Band",
                    SongTitle = "Still in Love",
                    AlbumTitle = "The Buck Starts Here",
                    CriticRating = 8.4,
                    Genre = "Rock",
                },
                new Music
                {
                    Artist = "The Acme Band",
                    SongTitle = "Lock Out, World",
                    AlbumTitle = "The Buck Starts Here",
                    CriticRating = 8.4,
                    Genre = "Rock",
                },
            };

            MusicRepository repo = new MusicRepository(DDBContext);
            await repo.BatchWriteAsync(writeList);

            List<Music> keyList = new List<Music>()
            {
                new Music
                {
                    Artist = "No One You Know",
                    SongTitle = "My Doc Spot",
                },
                new Music
                {
                    Artist = "No One You Know",
                    SongTitle = "Somewhere Down The Road",
                },
                new Music
                {
                    Artist = "The Acme Band",
                    SongTitle = "Lock Out, World",
                },
            };



            List<Music> batchGetList = await repo.BatchGetAsync(keyList);
            Assert.IsNotNull(batchGetList);
            Assert.AreEqual(batchGetList.Count, keyList.Count);


            keyList.ForEach((Music key) =>
            {

                Music batchGetEnitty = batchGetList.Where(w => w.Artist == key.Artist && w.SongTitle == key.SongTitle).FirstOrDefault();
                Assert.IsNotNull(batchGetEnitty);
            });

        }
    }
}
