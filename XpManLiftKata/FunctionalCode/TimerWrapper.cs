using System;
using System.Timers;
using Microsoft.Win32;

namespace XpMan.LiftKata.FunctionalCode
{
    public class TimerWrapper : ITimer
    {
        private readonly Timer _timer = new Timer();

        public void Start(double interval)
        {
            _timer.Interval = interval;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public bool AutoReset 
        {
            get { return _timer.AutoReset; }
            set { _timer.AutoReset = value; }
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        public virtual event ElapsedEventHandler Elapsed
        {
            add { this._timer.Elapsed += value; }
            remove { this._timer.Elapsed -= value; }
        }
    }
}