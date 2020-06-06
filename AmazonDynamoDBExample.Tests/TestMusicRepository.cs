using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AmazonDynamoDBExample.Entities;
using AmazonDynamoDBExample.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Tests
{
    [TestClass]
    public class TestMusicRepository
    {
        [TestMethod]
        public async Task TestMusicRepositoryAsync()
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

            MusicRepository repo = new MusicRepository(DDBContext);

            Music entity = new Music
            {
                Artist = "No One You Know",
                SongTitle = "My Doc Spot",
                AlbumTitle = "Hey Now",
                CriticRating = 8.4,
                Genre = "Country",
                Year = 1984
            };
            await repo.MergeEntityAsync(entity);


            Music entity2 = await repo.GetEntityAsync(entity.Artist, entity.SongTitle);
            Assert.IsNotNull(entity2);

            Assert.AreEqual(entity.Artist, entity2.Artist);
            Assert.AreEqual(entity.SongTitle, entity2.SongTitle);


            await repo.DeleteEntityAsync(entity.Artist, entity.SongTitle);
            Music entity3 = await repo.GetEntityAsync(entity.Artist, entity.SongTitle);
            Assert.IsNull(entity3);

        }

    }
}
