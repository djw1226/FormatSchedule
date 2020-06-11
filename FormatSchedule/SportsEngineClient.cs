using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace FormatSchedule
{
    public class SportsEngineController
    {
        private string accessCode = "";
        private static readonly HttpClient client = new HttpClient();
        private const string RefreshCode = "f3bdd675a2b653a9ed86f75f0ed85b1e";
        private const string ClientId = "323ebd282804753dfa6a80f0f6e8a7a8";
        private const string ClientSecret = "f4a87139576c37e8cc51ec67591484e2";
        private const string ApiSiteId = "36327";


        public SportsEngineController()
        {
            if (accessCode == string.Empty)
            {
                string refresh = "https://user.sportngin.com/oauth/token?grant_type=refresh_token&client_id=" + ClientId +
                   "&client_secret=" + ClientSecret + "&refresh_token=" + RefreshCode;
                var result = client.PostAsync(refresh, null).Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                accessCode = response["access_token"];
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessCode);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("NGIN-API-VERSION", "0.1");
            }


        }
        public string CreateEvent(LeagueAthleticEvent leagueEvent, string json)
        {
                   

            //string json = JsonConvert.SerializeObject(leagueEvent);
            string sportsEngineId;
            string url = "https://api.sportngin.com/events";
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = client.PostAsync(url, content).Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                sportsEngineId = response["id"];
                return sportsEngineId;
            }


        }

        public string UpdateEvent(LeagueAthleticEvent leagueEvent, string sportsEngineId, string json)
        {
            
           
            //string json = JsonConvert.SerializeObject(leagueEvent);
            string validId;
            string url = "https://api.sportngin.com/events/" + sportsEngineId;
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = client.PutAsync(url, content).Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                validId = response["id"];
                return validId;
            }




        }
    }
}
