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

    public static void C_CreateRoomHandler(PacketSession session, IMessage packet)
    {
        C_CreateRoom createRoom = packet as C_CreateRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        GameRoom room = RoomManager.Instance.Find(0);
        
        room.Push(room.LeaveRoom, player.Info.PlayerId);

        room = RoomManager.Instance.Add(createRoom.RoomName);
        RoomManager.Instance.Find(room.RoomId);
        room.Push(room.CreateRoom, player);

        Program.TickRoom(room, 50);
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterRoom = packet as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        GameRoom room = RoomManager.Instance.Find(enterRoom.RoomID);
        room.Push(room.EnterRoom, player, RoomType.GameRoom);
    }

    public static void C_RoomListHandler(PacketSession session, IMessage packet)
    {
        GameRoom room = RoomManager.Instance.Find(0);
        room.Push(room.SendRoomList);
    }

    public static void C_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        C_LeaveRoom leaveRoom = packet as C_LeaveRoom;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;

        GameRoom room = RoomManager.Instance.Find(player.Room.RoomId);
        room.Push(room.LeaveRoom, player.Info.PlayerId);

        room = RoomManager.Instance.Find(0);
        room.Push(room.EnterRoom, player, RoomType.Lobby);
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move move_PK = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.PlayerMove, player, move_PK);
    }

    public static void C_StopMoveHandler(PacketSession session, IMessage packet)
    {
        C_StopMove stopMove_PK = packet as C_StopMove;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        GameRoom room = player.Room;
        if (room == null)
            return;
        room.Push(room.StopMove, player, stopMove_PK);
    }
}

