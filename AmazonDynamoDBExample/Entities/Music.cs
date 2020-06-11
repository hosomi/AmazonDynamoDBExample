using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Globalization;
using TimeZoneConverter;

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

        [DynamoDBProperty("LastModified", typeof(DateConverter))]
        public DateTime LastModified { get; set; }

    }

    internal class DateConverter : IPropertyConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            var dateTime = entry?.AsString();
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            try
            {
                return TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(dateTime), TZConvert.GetTimeZoneInfo("Tokyo Standard Time")).DateTime;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Entity(DateTime) -> DynamoDB(S)
        /// </summary>
        /// <param name="value"><see cref="DateTime"/></param>
        /// <returns><see cref="string"/></returns>
        public DynamoDBEntry ToEntry(object value)
        {
            if (value == null)
            {
                return new DynamoDBNull();
            }
            if (value.GetType() != typeof(DateTime) && value.GetType() != typeof(DateTime?))
            {
                return new DynamoDBNull();
            }

            return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }
    }

}
