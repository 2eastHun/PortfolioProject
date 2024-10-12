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
    public interface IRoom
    {
        int RoomId { get; set; }
        string RoomName { get; set; }
        int PlayerCount { get; set; }

        void Push(Action action);
        void Init();

        void Update();
    }

    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        object _lock = new object();

        Dictionary<int, Rooms> _rooms = new Dictionary<int, Rooms>();
        int _roomId = 0;
        string _roomName = "null";

        /// <summary>방을 만드는 함수</summary>
        public T Add<T>(string roomName) where T : Rooms, new()
        {
            T room = new T();
            room.Push(room.Init);

            lock (_lock)
            {
                room.RoomId = _roomId;
                room.RoomName = roomName;
                _rooms.Add(_roomId, room);
                _roomId++;
            }

            return room;
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
        public T Find<T>(int roomId) where T : Rooms, new()
        {
            lock (_lock)
            {
                if (_rooms.TryGetValue(roomId, out Rooms room))
                    return room as T;
                else
                    return null;
            }
        }

        public List<Rooms> RoomList()
        {
            List<Rooms> roomList = new List<Rooms>();
            
            foreach (Rooms room in _rooms.Values)
            {
                if(room.RoomId != 0)
                    roomList.Add(room);
            }

            return roomList;
        }
    }
}
