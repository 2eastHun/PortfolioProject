using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public ClientSession Generate() //세션 생성
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId; //세션ID 발급

                ClientSession session = new ClientSession();
                session.SessionID = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Session ID: {sessionId} 세션 생성");

                return session;
            }
        }

        public ClientSession Find(int id)//세션ID를 통해 찾는 함수
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);

                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                Console.WriteLine($"{session.SessionID} 세션삭제");
                _sessions.Remove(session.SessionID);
                int sessionId = --_sessionId;
                session.SessionID = sessionId;
            }
        }
    }
}
