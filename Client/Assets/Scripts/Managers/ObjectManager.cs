using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public PlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public static ObjectManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlayerSpawn();
    }

    public static GameObjectType GetObjectTypeById(int id)
    {
        // [UNUSED(1)][TYPE(7)] ���� �� [UNUSED(1)]�� �����ϱ� ���ؼ� & 0x7F (& == �ݴ�)
        int type = (id >> 24) & 0x7F;

        return (GameObjectType)type;
    }

    /// <summary>�÷��̾� �����ϴ� �Լ�</summary>
    /// <param name="info">�������� ���� �÷��̾� ����</param>
    /// <param name="myPlayer">������ �÷��̾� ����</param>
    public void PlayerSpawn()
    {
        GameObject myPlayerPrefab = Resources.Load<GameObject>("Prefabs/DogPBR");
        
        GameObject enemyPlayerPrefab = Resources.Load<GameObject>("Prefabs/EmenmyPlayer");

        //GameObject player1Spawn = GameObject.Find("PlayerSpawner");
        //Debug.Log(player1Spawn);
        //Transform player2Spawn = GameObject.Find("PlayerSpawner").transform.Find("Player2Spawner");

        if (PlayerPrefs.GetInt("Player1ID") == NetworkManager.instance.MyPlayerID)
        {
            //GameObject myPlayer = Instantiate(myPlayerPrefab);
            //myPlayer.AddComponent<MyPlayerController>();

            ////myPlayer.transform.SetParent(player1Spawn);

            //GameObject enemyPlayer = Instantiate(enemyPlayerPrefab, player2Spawn.position, Quaternion.identity);
            //enemyPlayer.transform.SetParent(player2Spawn);


            //GameObject player1spawner = GameObject.Find("Player1Spawner").transform.Find("MyPlayer(Clone)").gameObject;
            //player1spawner.gameObject.AddComponent<MyPlayerController>();


        }
        //else if (player2ID == NetworkManager.instance.PlayerID)
        //{
        //    GameObject myPlayer = Instantiate(myPlayerPrefab, player2Spawn.position, Quaternion.identity);
        //    myPlayer.transform.SetParent(player2Spawn);

        //    GameObject enemyPlayer = Instantiate(enemyPlayerPrefab, player1Spawn.position, Quaternion.identity);
        //    enemyPlayer.transform.SetParent(player1Spawn);

        //    myPlayer.gameObject.AddComponent<MyPlayerController>();
        //}

    }


}
