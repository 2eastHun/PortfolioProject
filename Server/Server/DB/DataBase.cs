using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Server.Game.Room;
using static System.Net.WebRequestMethods;

namespace Test
{
    public class PlayerData
    {
        public int player_id { get; set; }
        public string player_name { get; set; }
        public int server_session { get; set; }
        public bool is_login { get; set; }
    }
    public class RoomData
    {
        public int room_id { get; set; }
        public string room_name { get; set; }
        public int player1_id { get; set; }
        public string player1_name { get; set; }
        public int player2_id { get; set; }
        public string player2_name { get; set; }
        public bool is_playing { get; set; }
    }

    public class DataBase
    {
        public static DataBase Instance { get; } = new DataBase();

        public static int requestID { get; set; }

        private static readonly HttpClient client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        public static async Task<string> Get(string url)
        {
            //HttpResponseMessage response = await client.GetAsync(url).Result;
            //response.EnsureSuccessStatusCode();
            //string responseBody = await response.Content.ReadAsStringAsync().Result;

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        public static async Task<dynamic> Deserialize(string path)
        {
            string url = path;
            string json = await Get(url);

            //json = JsonConvert.DeserializeObject<string>(json);
             return  JsonConvert.DeserializeObject(json);
        }
        public static async Task<dynamic> DeserializeGetParameter(string path, string parameter, dynamic value)
        {
            string url = path + "?" + parameter + "=" + value;
            string json = await Get(url);

            return JsonConvert.DeserializeObject(json);
        }

        public static async Task<string> Request(string func, dynamic data)
        {
            string url = $"https://localhost:7275/PlayerData/{func}";
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            //HttpResponseMessage response = client.PostAsync(url, content).Result;
            //response.EnsureSuccessStatusCode();

            //string responseBody = response.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(responseBody);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                //response.EnsureSuccessStatusCode().ReasonPhrase;
                return response.EnsureSuccessStatusCode().ReasonPhrase;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine("JSON 파싱 오류: " + ex.Message);

                return "error";
            }
        }

    }
}
