using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    public GameObject roomItemPrefab; // 방 아이템 프리팹
    public Transform content; // Scroll View의 Content

    private List<RoomInfo> _roomList;

    void Start()
    {
        C_RoomList roomList = new C_RoomList();
        NetworkManager.instance.Send(roomList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateRoomList()
    {

    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        if (roomList == null)
        {
            Debug.Log("RoomList is null");
            return;
        }

        _roomList = roomList;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _roomList.Count; i++)
        {
            RoomInfo roomInfo = roomList[i];
            GameObject roomItem = Instantiate(roomItemPrefab, content);
            roomItem.transform.Find("RoomName").GetComponent<TMP_Text>().text = roomInfo.Name;
            roomItem.transform.Find("Count").GetComponent<TMP_Text>().text = roomInfo.PlayerCount.ToString();

            UnityEngine.UI.Button enterButton = roomItem.GetComponent<UnityEngine.UI.Button>();
            
            int _index = i;
            enterButton.onClick.AddListener(() => OnEnterRoomButtonClick(_index));
        }
    }

    public void OnEnterRoomButtonClick(int index)
    {
        Debug.Log(index);
        Debug.Log($"Room ID: {_roomList[index].Id} Room Name: {_roomList[index].Name} Player Count: {_roomList[index].PlayerCount}");

        C_EnterRoom findRoomPacket = new C_EnterRoom();

        findRoomPacket.PlayerID = NetworkManager.instance.MyPlayerID;
        findRoomPacket.RoomID = _roomList[index].Id;

        NetworkManager.instance.Send(findRoomPacket);
    }
}