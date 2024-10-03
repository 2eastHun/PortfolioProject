using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        _roomList = roomList;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach(var room in roomList)
        {
            //RoomInfo roomInfo = roomList[i];
            GameObject roomItem = Instantiate(roomItemPrefab, content);
            roomItem.transform.Find("RoomName").GetComponent<TMP_Text>().text = room.Name;
            roomItem.transform.Find("Count").GetComponent<TMP_Text>().text = room.PlayerCount.ToString();
            Debug.Log(room.PlayerCount.ToString());
            //Button enterButton = roomItem.transform.Find("EnterButton").GetComponent<Button>();
            //Debug.Log("EnterButton : " + enterButton);
            //int index = i; // 로컬 변수로 인덱스 저장
            //enterButton.onClick.AddListener(() => OnEnterRoomButtonClick(index));
        }

        
    }

    public void EnterRoomButton()
    {
        
    }
}
