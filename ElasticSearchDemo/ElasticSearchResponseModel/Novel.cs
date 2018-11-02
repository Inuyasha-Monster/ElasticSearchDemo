using System;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElasticSearchDemo.ElasticSearchResponseModel
{
    [ElasticsearchType(Name = "noval")]
    public class Novel
    {
        [Text(Name = "title")]
        public string Title { get; set; }

        [Number(NumberType.Integer, Name = "word_count")]
        public int WordCount { get; set; }

        [Text(Name = "author")]
        public string Author { get; set; }

        [Date(Name = "publish_date", Format = "yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis")]
        [JsonConverter(typeof(DateTimeFormatConverter), "yyyy-MM-dd")]
        public DateTime PublishDate { get; set; }
    }

    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}