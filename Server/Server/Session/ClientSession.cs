using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.Object;
using Server.Game.Room;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public static ClientSession Instance { get; } = new ClientSession();

        //세션에 해당하는 플레이어
        public Player MyPlayer { get; set; }
        public int SessionID { get; set; }

        static ClientSession _session;
        string message;

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            // [size(2)][packetId(2)][ 데이터 ]
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];

            //sendBuffer 0번째 인덱스에 BitConverter.GetBytes(size+4) ([size(2))를 복사
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));

            //sendBuffer 2번째 인덱스에 BitConverter.GetBytes(protocolId) (packetID(2))를 복사
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));

            //sendBuffer 4번째 인덱스에 person.ToByteArray() (데이터)를 복사
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            //MyPlayer = ObjectManager.Instance.Add<Player>(0);
            //MyPlayer.Session = this;
            //MyPlayer.Info.Name = "None";


            //S_EnterGame enterGame = new S_EnterGame();

            //enterGame.Player = MyPlayer.Info;

            //Send(enterGame);

            
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)  //패킷 추출 하는 부분
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override async void OnDisconnected(EndPoint endPoint)
        {
            Lobby room = RoomManager.Instance.Find<Lobby>(MyPlayer.Room.RoomId);
            room.Push(room.LeaveRoom, MyPlayer);
            SessionManager.Instance.Remove(this);

            await OnLogout(this.MyPlayer.Id);

            Console.WriteLine($"{MyPlayer.Info.PlayerId}:Player {MyPlayer.Room.RoomId}:Room OnDisconnected: {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            // Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }

        public async Task OnLogin(string name)
        {
            dynamic playerData = await DataBase.DeserializeGetParameter("https://localhost:7275/PlayerData/GetPlayerByName", "name", name);

            Console.WriteLine(playerData.ToString());

            if (playerData.ToString() == "null")    
            {
                var dataCount = await DataBase.Deserialize("https://localhost:7275/PlayerData/GetDataCount");

                MyPlayer = ObjectManager.Instance.Add<Player>((int)dataCount + 1);
                MyPlayer.Session = this;
                MyPlayer.Info.Name = name;

                var requsetData = RequestData(MyPlayer.Info.Name, MyPlayer.Session.SessionID, true);

                message = await DataBase.Request("RequestLogin",requsetData);
                Console.WriteLine(message);
            }
            else
            {
                MyPlayer = ObjectManager.Instance.Add<Player>((int)playerData.player_id);
                MyPlayer.Session = this;
                MyPlayer.Info.Name = playerData.player_name;

                var requsetData = RequestData(MyPlayer.Info.Name, MyPlayer.Session.SessionID, true);

                Console.WriteLine($"{requsetData.player_name} {requsetData.server_session} {requsetData.is_login}");

                message = await DataBase.Request("RequestLogin",requsetData);
                Console.WriteLine(message);
            }

            if (message == "OK")
            {
                Rooms lobby = RoomManager.Instance.Find<Lobby>(0);
                lobby.Push(lobby.EnterRoom, MyPlayer);

                Console.WriteLine($"User ID:{MyPlayer.Info.PlayerId} Name:{MyPlayer.Info.Name} 유저 접속");

                S_LoginSuccess loginSuccess = new S_LoginSuccess();
                loginSuccess.Player = MyPlayer.Info;

                Send(loginSuccess);
            }
        }

        public async Task OnLogout(int playerID)
        {
            message = await DataBase.Request("RequestLogout", playerID);
            Console.WriteLine(message);
        }

        private dynamic RequestData(string playerName, int sessionId, bool isLogin)
        {
            return new
            {
                player_name = playerName,
                server_session = sessionId,
                is_login = isLogin
            };
        }

    }
}
