using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Room
{
    public class GameRoom : Rooms
    {
        Player[] _players = new Player[2];
 
        public override void EnterRoom(Player player)
        {
            if (PlayerCount >= 2)
                return;

            player.Room = this;
            player.Room.IsReady = false;

            player.Room.PlayerCount++;

            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i] == null)
                {
                    _players[i] = player;
                    break;
                }
            }

            S_EnterRoom EnterRoom = new S_EnterRoom();

            foreach (Player p in _players)
            {
                //if (player != p)
                EnterRoom.Player.Add(p.Info);
            }

            player.Session.Send(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Name} 플레이어 {this.RoomName}방 입장");
        }

        public void CreateGameRoom(Player player)
        {
            player.Room = this;

            player.Room.Info.PlayerCount += 1;
            player.Room.HostID = player.Id;
            player.Room.IsReady = false;

            _players[0] = player;

            //Lobby room = RoomManager.Instance.Find<Lobby>(0);

            S_EnterRoom EnterRoom = new S_EnterRoom();

            player.Session.Send(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Name} 플레이어 {this.RoomName}방 생성");
        }

        public override void LeaveRoom(int playerId)
        {
            //if (_players.Remove(playerId) == false)
            //    return;

            for(int i = 0; i<_players.Length; i++)
            {
                if (_players[i].Id == playerId)
                {
                    _players[i] = null;
                    break;
                }
            }

            this.PlayerCount--;


            if (this.RoomId != 0)
            {
                if (this.HostID == playerId)
                {
                    if (!_players.All(p => p == null))
                    {
                        foreach (Player p in _players)
                        {
                            if (p != null)
                            {
                                this.HostID = p.Id;
                                break;
                            }
                        }

                        S_LeaveRoom leaveRoom = new S_LeaveRoom();

                        foreach (Player p in _players)
                        {
                            //if (player != p)
                            leaveRoom.Player.Remove(p.Info);
                        }

                        Broadcast(leaveRoom);
                    }
                    else
                    {
                        RoomManager.Instance.Remove(this.RoomId);
                        Console.WriteLine("방에 유저가 없어서 방 삭제");
                    }
                }
            }

            Rooms room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{playerId} 번 플레이어 {this.RoomId}번 방에서 나감");
        }
        public override void Broadcast(IMessage packet)
        {
            foreach (Player p in _players)
            {
                p.Session.Send(packet);
            }
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

        public void LeaveLobby(int playerId)
        {
            //GameObjectType type = ObjectManager.GetObjectTypeById(objectId);
            
        }
    }
}
