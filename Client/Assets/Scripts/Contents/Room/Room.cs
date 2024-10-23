using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMyPlayerNameText(string name)
    {
        GameObject myPlayerName = GameObject.Find("Canvas").transform.Find("MyPlayerName").gameObject;
        myPlayerName.GetComponent<TMPro.TextMeshProUGUI>().text = name;
    }

    public void SetEnemyNameText(string name)
    {
        GameObject enemyPlayerName = GameObject.Find("Canvas").transform.Find("EnemyPlayerName").gameObject;
        enemyPlayerName.GetComponent<TMPro.TextMeshProUGUI>().text = name;
    }

    public void StartButton()
    {
        C_Start start = new C_Start();
        NetworkManager.instance.Send(start);
    }

    public void ReadyButton()
    {
        C_Ready ready = new C_Ready();
        NetworkManager.instance.Send(ready);
    }

    public void ExitButton()
    {
        C_LeaveRoom leaveRoom = new C_LeaveRoom();

        NetworkManager.instance.Send(leaveRoom);
    }
}
