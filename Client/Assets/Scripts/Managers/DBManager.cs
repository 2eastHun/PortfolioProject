using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

static class URL
{
    public static readonly string PlayerData = "https://localhost:7275/PlayerData";
    public static readonly string RoomData = "https://localhost:7275/RoomData";
}

public class DBManager : MonoBehaviour
{
    private static HttpClient client = null;
    public static DBManager instance = null;

    public struct PlayerData
    {
        public int player_id { get; set; }
        public string player_name { get; set; }
        public int server_session { get; set; }
        public bool is_login { get; set; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //List<PlayerData> playerDatas = Deserialize<PlayerData>("https://localhost:7275/PlayerData");

        //foreach (var data in playerDatas)
        //{
        //    Debug.Log($"ID:{data.player_id}, �̸�:{data.player_name}, SessionID:{data.server_session}, Login?:{data.is_login}");
        //}

       // StartCoroutine(GetPlayerData("https://localhost:7275/PlayerData/GetPlayer?playerID=14"));

    }
    //public void Login(string url, string playerName)
    //{
    //    StartCoroutine(PostPlayerData(url, playerName));
    //}

    //private IEnumerator PostPlayerData(string url, string playerName)
    //{
    //    // ������ �����͸� ��ü�� ����
    //    var playerData = new { playerName = playerName };

    //    // ��ü�� JSON ���ڿ��� ��ȯ
    //    string jsonData = JsonConvert.SerializeObject(playerData);

    //    // JSON ���ڿ��� ����Ʈ �迭�� ��ȯ
    //    byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

    //    // UnityWebRequest ��ü ����
    //    UnityWebRequest request = new UnityWebRequest(url, "POST");
    //    request.uploadHandler = new UploadHandlerRaw(postData);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");

    //    // ��û ����
    //    yield return request.SendWebRequest();

    //    // ���� ó��
    //    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //    {
    //        Debug.LogError(request.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Response: " + request.downloadHandler.text);
    //    }
    //}

    private IEnumerator GetPlayerData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json); // JSON ���ڿ� Ȯ��

            if (IsValidJson(json))
            {
                try
                {
                    // �̽��������� JSON ���ڿ��� ���ڵ�
                    string decodedJson = JsonConvert.DeserializeObject<string>(json);
                    List<PlayerData> playerDatas = JsonConvert.DeserializeObject<List<PlayerData>>(decodedJson);
                    foreach (var data in playerDatas)
                    {
                        Debug.Log($"ID:{data.player_id}, �̸�:{data.player_name}, SessionID:{data.server_session}, Login?:{data.is_login}");
                    }
                }
                catch (JsonSerializationException ex)
                {
                    Debug.LogError($"JSON ������ȭ ����: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError("Received JSON is not valid.");
            }
        }
    }

    public bool IsValidJson(string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError($"Invalid JSON: {ex.Message}");
            return false;
        }
    }

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
        client.BaseAddress = new Uri(url);
        HttpResponseMessage response = client.GetAsync("").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }
    public static List<T> Deserialize<T>(string path)
    {
        string url = path;
        string json = Get(url);

        json = JsonConvert.DeserializeObject<string>(json);
        return JsonConvert.DeserializeObject<List<T>>(json);
    }
    public static T DeserializeGetParameter<T>(string path, string parameter, T value)
    {
        string url = path + "?" + parameter + "=" + value;
        string json = Get(url);

        json = JsonConvert.DeserializeObject<string>(json);
        return JsonConvert.DeserializeObject<T>(json);
    }

    //public void Login(string path, string parameter, string name)
    //{
    //    Get(path + "?" + parameter + "=" + name);
    //}
}