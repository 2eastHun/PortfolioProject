﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    static public PlayerSpawn spawner { get; set; }

    public static void S_LoginSuccessHandler(PacketSession session, IMessage packet)
    {
        S_LoginSuccess login = packet as S_LoginSuccess;

        Debug.Log("로그인 성공");

        NetworkManager.instance.PlayerID = login.Player.PlayerId;
        NetworkManager.instance.PlayerName = login.Player.Name;

        SceneManager.LoadScene("Lobby");
    }

    public static void S_RoomListHandler(PacketSession session, IMessage packet)
    {
        S_RoomList roomList = packet as S_RoomList;


        GameObject gameObject = GameObject.Find("Canvas").transform.Find("RoomList").gameObject;

        RoomList list = gameObject.GetComponent<RoomList>();
        list.UpdateRoomList(roomList.Room.ToList());

        foreach (var room in roomList.Room)
        {
            Debug.Log($"{room.Id} {room.Name} {room.PlayerCount}");

        }
    }

    public static void S_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        S_EnterRoom enterLobby_PK = packet as S_EnterRoom;

        //foreach (var player in enterLobby_PK.Player)
        //{
        //    Debug.Log($"{player.PlayerId} {player.Name}");
        //}

        //NetworkManager.instance.PlayerID = enterGame_PK.Player.;

        //Debug.Log($"Player ID :{NetworkManager.instance.PlayerID}");
        //Debug.Log($"Player ID :{enterGamePacket.Player.Name}");
    }

    public static void S_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        S_LeaveRoom enterLobby_PK = packet as S_LeaveRoom;

        //foreach (var player in enterLobby_PK.Player)
        //{
        //    Debug.Log($"{player.PlayerId} {player.Name}");
        //}

        //NetworkManager.instance.PlayerID = enterGame_PK.Player.;

        //Debug.Log($"Player ID :{NetworkManager.instance.PlayerID}");
        //Debug.Log($"Player ID :{enterGamePacket.Player.Name}");
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGame_PK = packet as S_LeaveGame;

    }

    public static void S_EnterBattlefieldHandler(PacketSession session, IMessage packet)
    {
        S_EnterBattlefield enterBF_PK = packet as S_EnterBattlefield;

        SceneManager.LoadScene("Battlefield");

        PlayerPrefs.SetInt("Player1ID", enterBF_PK.Player1ID);
        PlayerPrefs.SetInt("Player2ID", enterBF_PK.Player2ID);
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
    //    S_Move move_PK = packet as S_Move;

    //    PlayerPrefs.SetInt("moving", 1);

    //    if (PlayerPrefs.GetInt("Player1ID") != NetworkManager.instance.PlayerID)
    //    {
    //        if (move_PK.PlayerID == NetworkManager.instance.PlayerID)
    //            return;

    //        GameObject enemyPlayer = GameObject.Find("PlayerSpawner").transform.Find("Player1Spawner").transform.Find("EnemyPlayer(Clone)").gameObject;

    //        EnemyPlayer MP = enemyPlayer.GetComponent<EnemyPlayer>();
    //        MP.PosFromServer(move_PK.PlayerID,move_PK.PosX, move_PK.PosY, move_PK.PosZ, move_PK.DirX, move_PK.DirY, move_PK.DirZ);
    //    }
    //    else
    //    {
    //        if (move_PK.PlayerID == NetworkManager.instance.PlayerID)
    //            return;

    //        GameObject enemyPlayer = GameObject.Find("PlayerSpawner").transform.Find("Player2Spawner").transform.Find("EnemyPlayer(Clone)").gameObject;

    //        EnemyPlayer MP = enemyPlayer.GetComponent<EnemyPlayer>();
    //        MP.PosFromServer(move_PK.PlayerID, move_PK.PosX, move_PK.PosY, move_PK.PosZ, move_PK.DirX, move_PK.DirY, move_PK.DirZ);
    //    }
    }

    public static void S_StopMoveHandler(PacketSession session, IMessage packet)
    {
    //    PlayerPrefs.SetInt("moving", 0);
    //    S_StopMove stopMove_PK = packet as S_StopMove;

    //    if (PlayerPrefs.GetInt("Player1ID") != NetworkManager.instance.PlayerID)
    //    {
    //        if (stopMove_PK.PlayerID == NetworkManager.instance.PlayerID)
    //            return;

    //        GameObject enemyPlayer = GameObject.Find("PlayerSpawner").transform.Find("Player1Spawner").transform.Find("EnemyPlayer(Clone)").gameObject;

    //        EnemyPlayer MP = enemyPlayer.GetComponent<EnemyPlayer>();
    //        MP.StopMove(stopMove_PK.PosX, stopMove_PK.PosY, stopMove_PK.PosZ);
    //    }
    //    else
    //    {
    //        if (stopMove_PK.PlayerID == NetworkManager.instance.PlayerID)
    //            return;

    //        GameObject enemyPlayer = GameObject.Find("PlayerSpawner").transform.Find("Player2Spawner").transform.Find("EnemyPlayer(Clone)").gameObject;

    //        EnemyPlayer MP = enemyPlayer.GetComponent<EnemyPlayer>();
    //        MP.StopMove(stopMove_PK.PosX, stopMove_PK.PosY, stopMove_PK.PosZ);
    //    }


    }

    public static void S_BroadcastTestHandler(PacketSession session, IMessage packet)
    {
        S_BroadcastTest testPacket = packet as S_BroadcastTest;

        GameObject.Find("Canvas").transform.Find("Test").gameObject.SetActive(true);
    }

    
}

