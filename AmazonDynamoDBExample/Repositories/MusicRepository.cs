using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AmazonDynamoDBExample.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Repositories
{
    public class MusicRepository
    {
        private readonly IDynamoDBContext DDBContext;
        private readonly IAmazonDynamoDB DDBClient;

        private MusicRepository() { }
        public MusicRepository(IDynamoDBContext DDBContext)
        {
            this.DDBContext = DDBContext;
        }

        public MusicRepository(IAmazonDynamoDB DDBClient)
        {
            this.DDBClient = DDBClient;
        }


        public async Task<Music> GetEntityAsync(string Artist, string SongTitle)
        {
            return await DDBContext.LoadAsync<Music>(Artist, SongTitle);
        }

        public async Task MergeEntityAsync(Music entity)
        {
            await DDBContext.SaveAsync(entity);
        }

        public async Task BatchWriteAsync(List<Music> list)
        {
            var writer = DDBContext.CreateBatchWrite<Music>();
            writer.AddPutItems(list);
            await writer.ExecuteAsync();
        }
        public async Task<List<Music>> BatchGetAsync(List<Music> keyList)
        {
            var getter = DDBContext.CreateBatchGet<Music>();
            keyList.ForEach((Music key) => {

                getter.AddKey(key.Artist, key.SongTitle);
            });

            await getter.ExecuteAsync();
            return getter.Results;
        }


        public async Task DeleteEntityAsync(string Artist, string SongTitle)
        {
            await DDBContext.DeleteAsync<Music>(Artist, SongTitle);
        }

        public async Task<GetItemResponse> GetItemAsync(string Artist, string SongTitle)
        {
            var request = new GetItemRequest
            {
                TableName = "Music",
                Key = new Dictionary<string, AttributeValue>() {
                    {"Artist", new AttributeValue { S = Artist } },
                    { "SongTitle", new AttributeValue { S = SongTitle } }
                }
            };

            return await DDBClient.GetItemAsync(request);
        }

        public async Task<PutItemResponse> PutItemAsync(Dictionary<string, AttributeValue> item)
        {
            return await DDBClient.PutItemAsync("Music", item);
        }


        public async Task<DeleteItemResponse> DeleteItemAsync(string Artist, string SongTitle)
        {
            var requestDelete = new DeleteItemRequest
            {
                TableName = "Music",
                Key = new Dictionary<string, AttributeValue>() {
                    {"Artist", new AttributeValue { S = Artist } },
                    { "SongTitle", new AttributeValue { S = SongTitle } }
                }
            };
            return await DDBClient.DeleteItemAsync(requestDelete);
        }
    }
}
