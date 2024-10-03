using Google.Protobuf.Protocol;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Room
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        object _lock = new object();

        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 0;
        string _roomName = "null";

        


        /// <summary>방을 만드는 함수</summary>
        public GameRoom Add(string roomName)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.Init);

            lock (_lock)
            {
                gameRoom.RoomId = _roomId;
                gameRoom.RoomName = roomName;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }

        /// <summary>방을 제거하는 함수</summary>
        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                _roomId--;
                return _rooms.Remove(roomId);
            }
        }

        /// <summary>방을 찾는 함수</summary> 
        /// <param name="roomId">찾을 방 ID</param>
        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;
                else
                    return null;
            }
        }

        public List<GameRoom> RoomList()
        {
            List<GameRoom> roomList = new List<GameRoom>();
            
            foreach (GameRoom room in _rooms.Values)
            {
                if(room.RoomId != 0)
                    roomList.Add(room);
            }

            return roomList;
        }
    }
}
