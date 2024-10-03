using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    internal class Program
    {
        private static HttpClient client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip |
                                    DecompressionMethods.Deflate
        });

        public static string Get(string url)
        {
            if (client == null)
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip |
                                            DecompressionMethods.Deflate
                };
                client = new HttpClient(handler);
            }
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            // 응답 내용을 출력하여 확인
            Console.WriteLine("Response Body:");
            Console.WriteLine(responseBody);

            return responseBody;
        }

        public static List<T> Deserialize<T>(string path)
        {
            string url = path;
            string json = Get(url);

            // JSON 응답 내용을 출력하여 확인
            Console.WriteLine("JSON Response:");
            Console.WriteLine(json);

            //json = JsonConvert.DeserializeObject<string>(json);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }



        public static dynamic DeserializeGetParameter(string path, string parameter, dynamic value)
        {
            string url = path + "?" + parameter + "=" + value;
            string json = Get(url);

            return JsonConvert.DeserializeObject(json);
        }

        public static void InsertDB<T>(string path, string parameter, dynamic value)
        {
            Get(path + "?" + parameter + "=" + value);
        }

        public static async Task InsertDBAsync(PlayerData playerData)
        {
            string url = "https://localhost:7275/PlayerData/InsertPlayer";
            string json = JsonConvert.SerializeObject(playerData);
            StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public static string RequestLogin(PlayerData playerData)
        {
            string url = "https://localhost:7275/PlayerData/RequestLogin";
            string json = JsonConvert.SerializeObject(playerData);
            StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response =  client.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();

            string responseBody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseBody);

            try
            {
                // JSON 문자열을 동적 객체로 변환
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // 필요한 값 추출
                string message = responseObject.message;

                return message;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine("JSON 파싱 오류: " + ex.Message);

                return "Error!";
            }
        }

        public static async Task<string> Request(string func,dynamic data)
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

        public static async Task Main(string[] args)
        {
            // PlayerData playerData = new PlayerData
            // {
            //     player_name = "2r2r2r",
            //     server_session = 132,
            //     is_login = false
            // };

            string message = await Request("RequestLogout", 26);

            Console.WriteLine(message);
            //dynamic playerDatas = DeserializeGetParameter("https://localhost:7275/PlayerData/GetPlayerByName","name","zxc");

            //Console.WriteLine(playerDatas.error.ToString());

            //foreach (var data in playerDatas)
            //{
            //Console.WriteLine($"ID:{playerDatas.player_id}, 이름:{playerDatas.player_name}, SessionID:{playerDatas.server_session}, Login?:{playerDatas.is_login}");
            //}
        }
    }
}