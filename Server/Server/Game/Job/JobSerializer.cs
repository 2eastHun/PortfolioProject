using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    /// <summary>당장 작업해야 되는 일들은 잡큐로 관리
    /// <para>나중에 실행해야 되는 일들은 잡타이머로 관리</para></summary>
    public class JobSerializer
    {
        /// <summary> 나중에 작업 해야되는 행동들 목록 </summary>
        JobTimer _timer = new JobTimer();

        /// <summary> 당장 작업 해야되는 행동들 목록 </summary>
        Queue<IJob> _jobQueue = new Queue<IJob>();

        object _lock = new object();

        // 실행 중인지 판별
        bool _flush = false;

        // PushAfter Helper 함수
        public void PushAfter(int tickAfter, Action action) { PushAfter(tickAfter, new Job(action)); }
        public void PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1) { PushAfter(tickAfter, new Job<T1>(action, t1)); }
        public void PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2) { PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2)); }
        public void PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3)); }

        /// <summary>
        /// JobTimer에 일감을 넣는 함수
        /// </summary>
        /// <param name="tickAfter">시간</param>
        /// <param name="job">일감</param>
        public void PushAfter(int tickAfter, IJob job)
        {
            _timer.Push(job, tickAfter);
        }



        // Push Helper 함수
        public void Push(Action action) { Push(new Job(action)); }
        public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

        /// <summary> JobQueue에 일감을 넣는 함수 </summary>
        public void Push(IJob job)
        {
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
            }
        }

        /// <summary> JobQueue, JobTimer에 쌓인 일감을 실행하는 함수 </summary>
        public void Flush()
        {
            _timer.Flush();

            while (true)
            {
                IJob job = Pop();

                if (job == null)
                    return;

                //JobQueue 일감을 실행
                job.Execute();
            }
        }

        /// <summary> JobQueue에 쌓인 일감을 가져오는 함수 </summary>
        IJob Pop()
        {
            lock (_lock)
            {
                //JobQueue 일감 실행 끝
                if (_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
