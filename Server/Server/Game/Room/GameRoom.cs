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
                EnterRoom.Player.Add(p.Info);
            }
            
            Broadcast(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.Name} 플레이어 {this.RoomName}방 입장");

            Console.WriteLine($"현재 입장한 플레이어{_players.Length}명");
            for (int i = 0; i<_players.Length; i++)
            {
                Console.WriteLine($"{_players[i].Info.Name}");
            }
        }

        public void CreateGameRoom(Player player)
        {
            player.Room = this;

            player.Room.Info.PlayerCount += 1;
            player.Room.HostID = player.Info.PlayerId;
            player.Room.IsReady = false;
            _players[0] = player;
            S_EnterRoom EnterRoom = new S_EnterRoom();

            
            EnterRoom.Player.Add(player.Info);
            player.Session.Send(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.Name} 플레이어 {this.RoomName}방 생성");
        }

        public override void LeaveRoom(Player player)
        {
            Player exitPlayer = player;


            //나가기 버튼 누른 유저에게 전송 - 방에서 나가라
            S_LeaveRoom leaveRoom = new S_LeaveRoom();
            leaveRoom.Player = exitPlayer.Info;
            player.Session.Send(leaveRoom);

            for (int i = 0; i<_players.Length; i++)
            {
                if (_players[i] != null && _players[i].Info.PlayerId == player.Info.PlayerId)
                {
                    _players[i] = null;
                    break;
                }
            }

            this.PlayerCount--;

            //방에 머물고 있는 유저에게 전송 - 적 유저 정보 삭제
            Player stayPlayer = _players.FirstOrDefault(p => p != null);
            if (stayPlayer != null)
            {
                S_LeaveRoom sendEnemyplayer = new S_LeaveRoom();
                sendEnemyplayer.Player = exitPlayer.Info;
                stayPlayer.Session.Send(sendEnemyplayer);
            }

            if (this.HostID == player.Info.PlayerId)
            {
                if (!_players.All(p => p == null))
                {
                    foreach (Player p in _players)
                    {
                        if (p != null)
                        {
                            this.HostID = p.Info.PlayerId;
                            break;
                        }
                    }
                }
                else
                {
                    RoomManager.Instance.Remove(this.RoomId);
                    Console.WriteLine("방에 유저가 없어서 방 삭제");
                }
            }

            Rooms room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.PlayerId} 번 플레이어 {this.RoomId}번 방에서 나감");
        }

        public override void Broadcast(IMessage packet)
        {
            foreach (Player p in _players)
            {
                if (p != null)
                    p.Session.Send(packet);
            }
        }
    }
}
