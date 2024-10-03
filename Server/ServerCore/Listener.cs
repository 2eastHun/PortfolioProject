using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;

        //세션을 어떤 방식으로 누구를 만들어 줄지를 정의
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            // 문지기                             //Ipv4                              //TCP
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _sessionFactory += sessionFactory;

            //문지기 교육
            _listenSocket.Bind(endPoint);

            //영업 시작   
            _listenSocket.Listen(backlog); //backlog: 최대 대기수
                                           //register: 문지기
            for (int i = 0; i < register; i++)
            {
                //Asysc작업이 완료되었을때 호출(나중에 호출) - 한번만 만들면 재사용 가능
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                //client가 커넥트 요청이 오면 콜백 방식으로 OnAcceptCompleted 호출
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args); //최초등록
            }
        }

        // 비동기: 동시에 처리되지 않고 나중에 처리될 수 있다
        //accept를 요청하는 부분과 실제로 처리가 되어 완료되는 부분을 따로 구현해야함

        //Register: 등록을 하다
        void RegisterAccept(SocketAsyncEventArgs args) //accept 요청을 등록하는 부분
        {
            args.AcceptSocket = null; //이벤트를 재 사용화 할때는 초기화 하는게 좋음

            //pending: 보류중                
            bool pending = _listenSocket.AcceptAsync(args); //client 요청이 들어올때까지 기다림-pending이 true인상태
            if (pending == false) //기다림이 끝남
                OnAcceptCompleted(null, args);
        }

        //이 콜백 함수는 별도의 쓰레드에서 실행됨(쓰레드 풀) 메인쓰레드와 이 함수가 동시다발적으로 같은 데이터를 건드리면 레이스컨티션 문제가 발생
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) //accept가 완료되었을때
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args); //완료가 됐으면 다음 client를 위해서 다시 등록
        }
    }
}
