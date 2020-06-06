using Amazon.DynamoDBv2.DataModel;
using AmazonDynamoDBExample.Entities;
using System.Threading.Tasks;

namespace AmazonDynamoDBExample.Repositories
{
    public class MusicRepository
    {
        private readonly IDynamoDBContext DDBContext;

        private MusicRepository() { }
        public MusicRepository(IDynamoDBContext DDBContext)
        {
            this.DDBContext = DDBContext;
        }

        public async Task<Music> GetEntityAsync(string Artist, string SongTitle)
        {
            return await DDBContext.LoadAsync<Music>(Artist, SongTitle);
        }

        public async Task MergeEntityAsync(Music entity)
        {
            await DDBContext.SaveAsync(entity);
        }

        public async Task DeleteEntityAsync(string Artist, string SongTitle)
        {
            await DDBContext.DeleteAsync<Music>(Artist, SongTitle);
        }
    }
}
