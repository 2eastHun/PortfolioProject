using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public TMP_InputField _roomName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPanelButton()
    {
        GameObject.Find("Canvas").transform.Find("CreateRoom").transform.Find("Panel").gameObject.SetActive(true);
    }
    public void CreateRoomButton()
    {
        C_CreateRoom createRoom = new C_CreateRoom();

        createRoom.PlayerID = NetworkManager.instance.MyPlayerID;
        createRoom.RoomName = _roomName.text;

        NetworkManager.instance.Send(createRoom);

        SceneManager.LoadScene("Room");
    }

    public void SetActiveButton()
    {
       GameObject.Find("Canvas").transform.Find("CreateRoom").transform.Find("Panel").gameObject.SetActive(false);
    }
}
