using Amazon.DynamoDBv2.DataModel;

namespace AmazonDynamoDBExample.Entities
{
    [DynamoDBTable("Music")]
    public class Music
    {
        [DynamoDBHashKey]
        public string Artist { get; set; }

        [DynamoDBRangeKey]
        public string SongTitle { get; set; }

        public string AlbumTitle { get; set; }

        public string Genre { get; set; }

        public double CriticRating { get; set; }

        public int Year { get; set; }
    }
}
