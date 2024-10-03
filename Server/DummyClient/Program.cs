using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;
using DummyClient;

namespace TestGameClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); //로컬 컴퓨터의 host 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); //최종주소 + 포트

            Connector connector = new Connector();                                     //돌릴 클라이언트 수
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); },500);

            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(250);
            }
        }
    }
}