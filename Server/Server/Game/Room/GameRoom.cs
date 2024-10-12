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
        public int RoomId { get { return Info.Id; } set { Info.Id = value; } }
        public string RoomName { get { return Info.Name; } set { Info.Name = value; } }
        public int PlayerCount { get { return Info.PlayerCount; } set { Info.PlayerCount = value; } }

        public int HostID { get { return Info.HostID; } set { Info.HostID = value; } }
        public bool IsReady { get { return Info.IsReady; } set { Info.IsReady = value; } }

        public RoomInfo Info { get; set; } = new RoomInfo();
        Player[] _players = new Player[2];
        TimeManager timeManager = new TimeManager();

        public void Init()
        {

        }

        public void Update()
        {
            Flush();
            timeManager.Update();
        }

        public override void EnterRoom(Player player)
        {
            if (PlayerCount >= 2)
                return;

            player.Room = this;

            player.Room.PlayerCount++;

            _players[player.Room.PlayerCount - 1] = player;

            S_EnterRoom EnterRoom = new S_EnterRoom();
            EnterRoom.RoomType = RoomType.GameRoom;

            player.Session.Send(EnterRoom);
        }

        public override void LeaveRoom(int playerId)
        {
            //throw new NotImplementedException();
        }

        public void CreateGameRoom(Player player)
        {
            //player.Room = this;

            //player.Room.Info.PlayerCount++;
            //player.Room.HostID = player.Id;
            //player.Room.IsReady = false;

            //_players.Add(player.Id, player);

            //Rooms room = RoomManager.Instance.Find<Rooms>(0);
            //room.SendRoomList();

            //S_EnterRoom EnterRoom = new S_EnterRoom();
            //EnterRoom.RoomType = RoomType.GameRoom;

            //player.Session.Send(EnterRoom);
        }
    }
}
