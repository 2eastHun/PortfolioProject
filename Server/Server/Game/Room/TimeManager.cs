using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Room
{
    public class TimeManager
    {
        private Stopwatch stopwatch;
        private double lastFrameTime;
        public float DeltaTime { get; private set; }

        public TimeManager()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            lastFrameTime = stopwatch.Elapsed.TotalSeconds;
        }

        public void Update()
        {
            double currentFrameTime = stopwatch.Elapsed.TotalSeconds;
            DeltaTime = (float)(currentFrameTime - lastFrameTime);
            lastFrameTime = currentFrameTime;
        }
    }
}
