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
            throw new NotImplementedException();
        }

        public override void LeaveRoom(int playerId)
        {
            throw new NotImplementedException();
        }

        public void EnterLobby(Player player)
        {
            player.Room = this;
            player.Room.PlayerCount++;

            _players.Add(player.Id, player);

            S_EnterRoom EnterLobby_PK = new S_EnterRoom();

            foreach (Player p in _players.Values)
            {
                //if (player != p)
                EnterLobby_PK.Player.Add(p.Info);
            }

            Broadcast(EnterLobby_PK);
            SendRoomList();

            Console.WriteLine($"{player.Id} 번 플레이어 로비 입장");
        }

        public void LeaveLobby(int playerId)
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

            Lobby room = RoomManager.Instance.Find<Lobby>(0);
            room.SendRoomList();

            if (this.RoomId != 0)
                Console.WriteLine($"{playerId} 번 플레이어 {this.RoomId}번 방에서 나감");
            else
                Console.WriteLine($"{playerId} 번 플레이어 로비 나감");
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
    }

}
