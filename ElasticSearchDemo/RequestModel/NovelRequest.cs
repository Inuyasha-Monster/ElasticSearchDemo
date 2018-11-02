using System;
using System.ComponentModel.DataAnnotations;
using ElasticSearchDemo.ElasticSearchResponseModel;
using Nest;
using Newtonsoft.Json;

namespace ElasticSearchDemo.RequestModel
{
    public class NovelRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Title { get; set; }


        public int WordCount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(10)]
        public string Author { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter), "yyyy-MM-dd")]
        public DateTime PublishDate { get; set; }
    }
}