using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormatSchedule
{
    class SportsEngineGame
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("page_node_ids")]
        public int Team1ID { get; set; }
        public int Team2ID { get; set; }

        public int SubseasonId { get; set; }
        public int SportId { get; set; }

        [JsonProperty("start_date_time")]
        public string StartDateTime { get; set; }
        [JsonProperty("end_date_time")]
        public string EndDateTime { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonIgnore]
        public int LeagueAthleticID { get; set; }
        public string EventType { get; set; }
    }
}
