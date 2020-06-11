using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormatSchedule
{
    
    public class LeagueAthleticEvent
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("page_node_ids")]
        public int PageNodeId { get; set; }
        [JsonProperty("start_date_time")]
        public string StartDateTime { get; set; }
        [JsonProperty("end_date_time")]
        public string EndDateTime { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonIgnore]
        public int LeagueAthleticID { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
    }
}
