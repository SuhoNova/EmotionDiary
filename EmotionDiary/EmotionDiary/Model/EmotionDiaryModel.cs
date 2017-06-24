using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EmotionDiary.Model
{
    public class EmotionDiaryModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "EmotionResult")]
        public string EmotionResult { get; set; }

        [JsonProperty(PropertyName = "EmotionScore")]
        public double EmotionScore { get; set; }

        [JsonProperty(PropertyName = "Longitude")]
        public float Longitude { get; set; }

        [JsonProperty(PropertyName = "Latitude")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        public string City { get; set; }
    }
}
