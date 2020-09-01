using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FormatSchedule
{
    public class SportsEngineController
    {
        private string accessCode = "";
        private static readonly HttpClient client = new HttpClient();
        private string RefreshCode
        {
            get
            {
                if (refreshCode == string.Empty)
                {
                    refreshCode = ConfigurationManager.AppSettings["RefreshCode"];
                }
                return refreshCode;
            }
        }
        private string refreshCode = "";
        private string clientId = "";
        private string clientSecret = "";
        private string apiSiteId = "";
        private string ClientId
        {
            get
            {
                if (clientId == string.Empty)
                {
                    clientId = ConfigurationManager.AppSettings["ClientId"];
                }
                return clientId;
            }
        }
        private string ClientSecret
        {
            get
            {
                if (clientSecret == string.Empty)
                {
                    clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
                }
                return clientSecret;
            }
        }
        private string ApiSiteId
        {
            get
            {
                if (apiSiteId == string.Empty)
                {
                    apiSiteId = ConfigurationManager.AppSettings["ApiSiteId"];
                }
                return apiSiteId;
            }
        }


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

        public string CancelEvent(LeagueAthleticEvent leagueEvent, string sportsEngineId)
        {


            //string json = JsonConvert.SerializeObject(leagueEvent);
            string validId;
            string url = "https://api.sportngin.com/events/" + sportsEngineId;

            HttpResponseMessage result = client.DeleteAsync(url).Result;
            var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
            validId = response["id"];
            return validId;





        }

        public string CreateGame(string json)
        {


            //string json = JsonConvert.SerializeObject(leagueEvent);
            string sportsEngineId;
            string url = "https://api.sportngin.com/games";
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = client.PostAsync(url, content).Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                sportsEngineId = response["id"];
                return sportsEngineId;
            }


        }
        public string UpdateGame(string sportsEngineId, string json)
        {


            //string json = JsonConvert.SerializeObject(leagueEvent);
            string validId;
            string url = "https://api.sportngin.com/games/" + sportsEngineId;
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage result = client.PutAsync(url, content).Result;
                var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
                validId = response["id"];
                return validId;
            }
        }

        public string CancelGame(string sportsEngineId)
        {


            //string json = JsonConvert.SerializeObject(leagueEvent);
            string validId;
            string url = "https://api.sportngin.com/games/" + sportsEngineId;

            HttpResponseMessage result = client.DeleteAsync(url).Result;
            var response = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);
            validId = response["id"];
            return validId;





        }
    }
}
