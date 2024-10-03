using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Schema;
using Google.Protobuf;
using Server.Game;
using Server.Game.Room;
using ServerCore;

namespace Server
{
    internal class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

        static public void TickRoom(GameRoom room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            //몇 틱마다 실행 할지 설정
            timer.Interval = tick;
            //interval이 끝나면 어떤 작업을 할지 설정
            timer.Elapsed += ((s, e) => { room.Update(); });
            //자동 실행
            timer.AutoReset = true;
            timer.Enabled = true;

            _timers.Add(timer);
        }

        static void Main(string[] args)
        {
            GameRoom room = RoomManager.Instance.Add("Lobby");
            TickRoom(room, 50);

            string host = Dns.GetHostName(); //로컬 컴퓨터의 host 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //최종주소 + 포트
                                                                //
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); }); //sessionManager
            Console.WriteLine("Listening...");

            //JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                //JobTimer.Instance.Flush();
                Thread.Sleep(100);
            }
        }
    }
}