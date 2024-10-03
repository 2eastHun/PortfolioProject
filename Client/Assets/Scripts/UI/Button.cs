using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update

    public void CreateRoom()
    {
        C_CreateRoom createRoom = new C_CreateRoom();

        createRoom.PlayerID = NetworkManager.instance.PlayerID;

        NetworkManager.instance.Send(createRoom);

        SceneManager.LoadScene("Room");
    }

    public void Test()
    {
        C_EnterRoom findRoomPacket = new C_EnterRoom();

        findRoomPacket.PlayerID = NetworkManager.instance.PlayerID;

        NetworkManager.instance.Send(findRoomPacket);
    }

    public void Test2()
    {
        C_CreateRoom createRoom = new C_CreateRoom();

        createRoom.PlayerID = NetworkManager.instance.PlayerID;

        NetworkManager.instance.Send(createRoom);

        SceneManager.LoadScene("Test");

        
    }
}
