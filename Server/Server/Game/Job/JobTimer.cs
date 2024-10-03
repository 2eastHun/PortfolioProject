using ServerCore;

namespace Server.Game
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        /// <summary>실행시간</summary>
        public int execTick;

        public IJob job;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        MyPriorityQueue<JobTimerElem> _pq = new MyPriorityQueue<JobTimerElem>();
        object _lock = new object();

        /// <summary> PriorityQueue에 데이터를 넣는 함수 </summary>
        /// <param name="action">작업해야 되는 일감</param>
        /// <param name="tickAfter">몇 틱후에 실행해야 되는지</param>
        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElement;
            jobElement.execTick = System.Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }

        /// <summary> PriorityQueue를 실행하는 함수 </summary>
        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    _pq.Pop();
                }

                jobElement.job.Execute();
            }
        }
    }
}
