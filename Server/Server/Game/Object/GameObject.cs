using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    //public struct Vector3
    //{
    //    public float x, y, z;
    //    public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    //}

    public class Player
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.PlayerId; }
            set { Info.PlayerId = value; }
        }

        public string Name
        {
            get { return Info.Name; }
            set { Info.Name = value; }
        }

        public ClientSession Session { get; set; }

        public Vector3 StartingPos { get; set; }

        public Vector3 Pos { get; set; }
        public Vector3 Dir { get; set; }



        /// <summary>입장한 방 정보</summary>
        public Rooms Room { get; set; }

        /// <summary>플레이어 정보</summary>
        public PlayerInfo Info { get; set; } = new PlayerInfo();

    }
}
