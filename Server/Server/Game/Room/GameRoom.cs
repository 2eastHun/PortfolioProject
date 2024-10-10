using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Object;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game
{
    //public enum RoomType
    //{
    //    Lobby,
    //    GameRoom
    //}

    public class GameRoom : JobSerializer
    {
        public int RoomId { get { return Info.Id; } set { Info.Id = value; } }
        public string RoomName { get { return Info.Name; } set { Info.Name = value; } }
        public int PlayerCount { get { return Info.PlayerCount; } set { Info.PlayerCount = value; } }

        public int HostID { get { return Info.HostID; } set { Info.HostID = value; } }
        public bool IsReady { get { return Info.IsReady; } set { Info.IsReady = value; } }

        public RoomInfo Info { get; set; } = new RoomInfo();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        TimeManager timeManager = new TimeManager();


        public void Init()
        {

        }

        public void Update()
        {
            Flush();
            timeManager.Update();
        }

        //public void EnterLobby(Player player)
        //{
        //    player.Room = this;
        //    player.Room.PlayerCount++;

        //    _players.Add(player.Id, player);

        //    S_EnterRoom EnterLobby_PK = new S_EnterRoom();

        //    foreach (Player p in _players.Values)
        //    {
        //        //if (player != p)
        //        EnterLobby_PK.Player.Add(p.Info);
        //    }

        //    Broadcast(EnterLobby_PK);
        //    SendRoomList();

        //    Console.WriteLine($"{player.Id} 번 플레이어 로비 입장");
        //}

        public void CreateRoom(Player player)
        {
            player.Room = this;

            player.Room.Info.PlayerCount++;
            player.Room.HostID = player.Id;
            player.Room.IsReady = false;

            _players.Add(player.Id, player);

            GameRoom room = RoomManager.Instance.Find(0);
            room.SendRoomList();

            S_EnterRoom EnterRoom = new S_EnterRoom();
            
            player.Session.Send(EnterRoom);
        }

        public void EnterRoom(Player player, RoomType roomType)
        {
            player.Room = this;
            player.Room.Info.PlayerCount++;
            player.Room.IsReady = false;

            _players.Add(player.Id,player);

            Console.WriteLine($"방 정보:{player.Room.RoomId}{player.Room.RoomName}{player.Room.PlayerCount}");

            S_EnterRoom enterRoom = new S_EnterRoom();

            foreach (Player p in _players.Values)
            {
                //if (player != p)
                enterRoom.Player.Add(p.Info);
            }

            if(roomType == RoomType.GameRoom)
                enterRoom.RoomType = RoomType.GameRoom;

            Broadcast(enterRoom);

            if (roomType == RoomType.GameRoom)
            {
                GameRoom room = RoomManager.Instance.Find(0);
                room.SendRoomList();
            }

            if (roomType == RoomType.GameRoom)
            {
                Console.WriteLine($"{player.Id} 번 플레이어 {this.RoomId}번 방 입장");
                //S_EnterRoom s_EnterRoom = new S_EnterRoom();
                //player.Session.Send(s_EnterRoom);
            }
            else
                Console.WriteLine($"{player.Id} 번 플레이어 로비 입장");
        }

        public void LeaveRoom(int playerId)
        {
            //GameObjectType type = ObjectManager.GetObjectTypeById(objectId);
            if (_players.Remove(playerId) == false)
                return;

            if (this.RoomId != 0)
            {
                if (this.HostID == playerId)
                {
                    if (_players.Count() > 0)
                    {
                        foreach (Player p in _players.Values)
                        {
                            this.HostID = p.Id;
                            break;
                        }
                    }
                    else
                    {
                        this.HostID = 0;
                    }
                }
            }

            this.PlayerCount--;

            S_LeaveRoom leaveRoom = new S_LeaveRoom();

            foreach (Player p in _players.Values)
            {
                //if (player != p)
                leaveRoom.Player.Remove(p.Info);
            }

            if (this.RoomId != 0 && _players.Count() == 0)
            {
                RoomManager.Instance.Remove(this.RoomId);
            }

            Broadcast(leaveRoom);

            GameRoom room = RoomManager.Instance.Find(0);
            room.SendRoomList();

            if (this.RoomId != 0)
                Console.WriteLine($"{playerId} 번 플레이어 {this.RoomId}번 방에서 나감");
            else
                Console.WriteLine($"{playerId} 번 플레이어 로비 나감");
        }

        //public void EnterRoom(Player player)
        //{
        //    //if (gameObject == null)
        //    //    return;

        //    //Player player = gameObject as Player;

        //    _players.Add(player.Id, player);
        //    player.Room = this;

        //    S_EnterRoom EnterLobby_PK = new S_EnterRoom();

        //    foreach (Player p in _players.Values)
        //    {
        //        //if (player != p)
        //        EnterLobby_PK.Player.Add(p.Info);
        //    }

        //    Broadcast(EnterLobby_PK);

        //    //foreach (var p  in _players)
        //    {
        //        Console.WriteLine($"{RoomId}번 방: {player.Id}번 플레이어 입장 ");
        //    }


        //    //todo 모든 플레이어가 레디 상태일때 방장이 시작버튼 누르면

        //    //S_EnterBattlefield EnterBF_PK = new S_EnterBattlefield();


        //    //if (_players.Count >= 1)
        //    //{
        //    //    EnterBF_PK.Player1ID = _players[0].Id;

        //    //    EnterBF_PK.Player2ID = _players[1].Id;

        //    //    Broadcast(EnterBF_PK);
        //    //}

        //    //EnterGame(player);
        //}

        


        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);
        }

        public void PlayerMove(Player player, C_Move move_PK)
        {
            //Console.WriteLine($"전 {player.StartingPos.X},{player.StartingPos.Y},{player.StartingPos.Z}");


            //float deltaTime = timeManager.DeltaTime;
            //player.StartingPos += new Vector3(-1,0,0) * 6.0f;

            //Console.WriteLine($"후 {player.StartingPos.X},{player.StartingPos.Y},{player.StartingPos.Z}");


            // 현재 위치를 가져오기
            Vector3 currentPosition = new Vector3(move_PK.PosInfo.PosX, move_PK.PosInfo.PosY, move_PK.PosInfo.PosZ);

            // 이동 벡터 계산
            Vector3 moveDirection = new Vector3(move_PK.PosInfo.MoveDirPosX, move_PK.PosInfo.MoveDirPosY, move_PK.PosInfo.MoveDirPosZ);

            // 시간 기반 보정
            Vector3 displacement = moveDirection /** 6.0f*/ /**move_PK.PosInfo.KeyPressDuration*/;

            // 새로운 위치 계산
            Vector3 newPosition = currentPosition;

            // 경계 조건 및 충돌 처리 (예시)
            //if (IsWithinBounds(newPosition) && !IsColliding(newPosition))
            {
                // 위치 업데이트
                player.Pos = newPosition;
                player.Dir = moveDirection;
            }

            // 클라이언트에 새 위치 전송
            //SendPositionToClient(player.Id, newPosition);
            S_Move s_move_PK = new S_Move();
            s_move_PK.PlayerID = player.Id;
            s_move_PK.PosX = player.Pos.X;
            s_move_PK.PosY = player.Pos.Y;
            s_move_PK.PosZ = player.Pos.Z;
            s_move_PK.DirX = player.Dir.X;
            s_move_PK.DirY = player.Dir.Y;
            s_move_PK.DirZ = player.Dir.Z;

            Console.WriteLine($"{player.Id}: ({player.Pos.X},{player.Pos.Y},{player.Pos.Z})");

            Broadcast(s_move_PK);
        }
        public void StopMove(Player player, C_StopMove stopMove_PK)
        {
            Console.WriteLine($"클라에서 보낸 StopMove");

            S_StopMove s_StopMove_PK = new S_StopMove();
            s_StopMove_PK.PlayerID = player.Id;
            s_StopMove_PK.PosX = player.Pos.X;
            s_StopMove_PK.PosY = player.Pos.Y;
            s_StopMove_PK.PosZ = player.Pos.Z;

            Broadcast(s_StopMove_PK);
        }

        public void SendRoomList()
        {
            if (this.RoomId != 0)
                return;

            List<RoomInfo> roomList = new List<RoomInfo>();

            foreach (GameRoom room in RoomManager.Instance.RoomList())
            {
                roomList.Add(room.Info);

                Console.WriteLine($"{room.RoomId}{room.RoomName}{room.PlayerCount}");
            }
            
            S_RoomList roomList_PK = new S_RoomList();
            roomList_PK.Room.AddRange(roomList);

            Broadcast(roomList_PK);
        }

        public void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
    }
}
