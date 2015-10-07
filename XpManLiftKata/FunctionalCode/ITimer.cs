using System;
using System.Timers;
using Microsoft.Win32;

namespace XpMan.LiftKata.FunctionalCode
{
    public interface ITimer
    {
        void Start(double interval);
        void Stop();
        bool AutoReset { get; set; }
        bool Enabled { get; set; }
        void Dispose();
        //event EventHandler<TimeElapsedEventArgs> Elapsed;
        event ElapsedEventHandler Elapsed;
    }

    public class TimeElapsedEventArgs : EventArgs
    {
        public DateTime SignalTime { get; private set; }

        public TimeElapsedEventArgs()
            : this(DateTime.Now)
        {
        }

        public TimeElapsedEventArgs(DateTime signalTime)
        {
            this.SignalTime = signalTime;
        }
    }
}