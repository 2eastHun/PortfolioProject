using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using Server.Game.Room;
using Server.Game.Object;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test;

/// <summary>
/// 클라이언트에서 온 패킷을 조립하는 클래스
/// </summary>
internal class PacketHandler
{
    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = packet as C_Login;
        ClientSession clientSession = session as ClientSession;

        clientSession.OnLogin(loginPacket.PlayerName);
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterRoom = packet as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        Rooms room = RoomManager.Instance.Find<Lobby>(0);
        room.Push(room.LeaveRoom, player);

        room = RoomManager.Instance.Find<GameRoom>(enterRoom.RoomID);
        room.Push(room.EnterRoom, player);
    }

    public static void C_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        C_LeaveRoom leaveRoom = packet as C_LeaveRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        Rooms room = RoomManager.Instance.Find<GameRoom>(player.Room.RoomId);
        room.Push(room.LeaveRoom, player);

        room = RoomManager.Instance.Find<Lobby>(0);
        room.Push(room.EnterRoom, player);
    }

    public static void C_CreateRoomHandler(PacketSession session, IMessage packet)
    {
        C_CreateRoom createRoom = packet as C_CreateRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        Rooms lobby = RoomManager.Instance.Find<Lobby>(0);
        lobby.Push(lobby.LeaveRoom, player);

        GameRoom gameRoom = RoomManager.Instance.Add<GameRoom>(createRoom.RoomName);
        RoomManager.Instance.Find<GameRoom>(gameRoom.RoomId);
        gameRoom.Push(gameRoom.CreateGameRoom, player);

        Program.TickRoom(gameRoom, 50);
    }

   

    public static void C_RoomListHandler(PacketSession session, IMessage packet)
    {
        Lobby room = RoomManager.Instance.Find<Lobby>(0);
        room.Push(room.SendRoomList);
    }

    public static void C_ReadyHandler(PacketSession session, IMessage packet)
    {
        C_Ready ready = packet as C_Ready;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        GameRoom room = RoomManager.Instance.Find<GameRoom>(player.Room.RoomId);
        room.Push(room.Ready, player);
    }

    public static void C_StartHandler(PacketSession session, IMessage packet)
    {
        C_Start start = packet as C_Start;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        GameRoom room = RoomManager.Instance.Find<GameRoom>(player.Room.RoomId);
        room.Push(room.Start, player);
    }

    //public static void C_ReadyCancelHandler(PacketSession session, IMessage packet)
    //{
    //    C_ReadyCancel readyCancel = packet as C_ReadyCancel;
    //    ClientSession clientSession = session as ClientSession;

    //    Player player = clientSession.MyPlayer;

    //    GameRoom room = RoomManager.Instance.Find<GameRoom>(player.Room.RoomId);
    //    room.Push(room.Ready, player);

    //}



    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
    //    C_Move move_PK = packet as C_Move;
    //    ClientSession clientSession = session as ClientSession;

    //    Player player = clientSession.MyPlayer;
    //    if (player == null)
    //        return;

    //    Lobby room = player.Room;
    //    if (room == null)
    //        return;

    //    room.Push(room.PlayerMove, player, move_PK);
    }

    public static void C_StopMoveHandler(PacketSession session, IMessage packet)
    {
    //    C_StopMove stopMove_PK = packet as C_StopMove;
    //    ClientSession clientSession = session as ClientSession;

    //    Player player = clientSession.MyPlayer;
    //    Lobby room = player.Room;
    //    if (room == null)
    //        return;
    //    room.Push(room.StopMove, player, stopMove_PK);
    }
}

