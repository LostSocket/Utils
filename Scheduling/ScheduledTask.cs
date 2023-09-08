using System;

namespace Utils.Scheduling
{
    public class ScheduledTask
    {
        public Action ToExecute;
        public float Delay;
        public float Interval;
        public int RepeatCount;
        public int ExecutedCount;

        public bool IsCancelled { get; private set; }

        public void Cancel()
        {
            if (!IsCancelled)
            {
                IsCancelled = true;
            }
        }
    }
}