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
    public class Lobby : Rooms
    {
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public override void EnterRoom(Player player)
        {
            player.Room = this;
            player.Room.PlayerCount++;

            _players.Add(player.Info.PlayerId, player);

            //S_EnterLobby EnterLobby_PK = new S_EnterLobby();

            //foreach (Player p in _players.Values)
            //{
            //    //if (player != p)
            //    EnterLobby_PK.Player.Add(p.Info);
            //}

            //Broadcast(EnterLobby_PK);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            
            room.SendRoomList();

            Console.WriteLine($"{player.Info.PlayerId} 번 플레이어 로비 입장");
        }

        public override void LeaveRoom(Player player)
        {
            if (_players.Remove(player.Info.PlayerId) == false)
                return;

            this.PlayerCount--;

            //S_LeaveRoom leaveRoom = new S_LeaveRoom();

            //foreach (Player p in _players.Values)
            //{
            //    //if (player != p)
            //    leaveRoom.Player.Remove(p.Info);
            //}

            //Broadcast(leaveRoom);

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            Console.WriteLine($"{player.Info.PlayerId} 번 플레이어 로비 나감");
        }

        public override void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
    }
}
