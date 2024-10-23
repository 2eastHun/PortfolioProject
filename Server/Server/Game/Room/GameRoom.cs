using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Room
{
    public class GameRoom : Rooms
    {
        //Player[] _players = new Player[2];
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public override void EnterRoom(Player player)
        {
            if (PlayerCount >= 2)
                return;

            player.Room = this;
            player.Room.PlayerCount++;
            _players.Add(player.Info.PlayerId, player);

            S_EnterRoom EnterRoom = new S_EnterRoom();

            foreach (Player p in _players.Values)
            {
                EnterRoom.Player.Add(p.Info);
            }

            EnterRoom.Room = player.Room.Info;

            Broadcast(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.Name} 플레이어 {this.RoomName}방 입장");

            Console.WriteLine($"현재 입장한 플레이어{_players.Count}명");
            for (int i = 0; i<_players.Count; i++)
            {
                Console.WriteLine($"{_players[i].Info.Name}");
            }
        }

        public override void LeaveRoom(Player player)
        {
            Player exitPlayer = player;

            //나가기 버튼 누른 유저에게 전송 - 방에서 나가라
            S_LeaveRoom leaveRoom = new S_LeaveRoom();
            leaveRoom.Player = exitPlayer.Info;
            player.Session.Send(leaveRoom);

            _players.Remove(player.Info.PlayerId);
            this.PlayerCount--;

            //방에 머물고 있는 유저에게 전송 - 적 유저 정보 삭제
            Player stayPlayer = _players.Values.FirstOrDefault(p => p != null);
            
            if (stayPlayer != null)
            {
                S_LeaveRoom sendEnemyplayer = new S_LeaveRoom();
                sendEnemyplayer.Player = exitPlayer.Info;
                stayPlayer.Session.Send(sendEnemyplayer);

                if (stayPlayer.Info.PlayerId != this.HostID)
                {
                    //기존 유저에게 방장권한 부여
                    this.HostID = stayPlayer.Info.PlayerId;
                    S_NewHost newHost = new S_NewHost();
                    stayPlayer.Session.Send(newHost);
                }
            }
            else
            {
                RoomManager.Instance.Remove(this.RoomId);
                Console.WriteLine("방에 유저가 없어서 방 삭제");
            }

            Rooms room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.PlayerId} 번 플레이어 {this.RoomId}번 방에서 나감");
        }

        public void CreateGameRoom(Player player)
        {
            player.Room = this;

            player.Room.Info.PlayerCount += 1;
            player.Room.HostID = player.Info.PlayerId;
            _players.Add(player.Info.PlayerId,player);

            S_EnterRoom EnterRoom = new S_EnterRoom();

            EnterRoom.Player.Add(player.Info);
            EnterRoom.Room = player.Room.Info;
            player.Session.Send(EnterRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.Name} 플레이어 {this.RoomName}방 생성");
        }

        public void Ready(Player player)
        {
            if (_players.TryGetValue(player.Info.PlayerId, out Player targetPlayer))
            {
                if(targetPlayer.Info.IsReady == false)
                {
                    targetPlayer.Info.IsReady = true;

                    S_Ready ready = new S_Ready();
                    ready.IsReady = true;
                    player.Session.Send(ready);
                }
                else
                {
                    targetPlayer.Info.IsReady = false;

                    S_Ready ready = new S_Ready();
                    ready.IsReady = false;
                    player.Session.Send(ready);
                }
            }  
        }

        public void Start(Player player)
        {
            Player guestPlayer = _players.Values.FirstOrDefault(p => p.Info.PlayerId != this.HostID);

            if(guestPlayer.Info.IsReady == true)
            {
                S_EnterBattlefield enterBattlefield = new S_EnterBattlefield();
                Broadcast(enterBattlefield);
            }
            else
            {
                S_Start start = new S_Start();
                player.Session.Send(start);
            }
        }

        public override void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                if (p != null)
                    p.Session.Send(packet);
            }
        }
    }
}
