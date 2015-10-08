using System;
using System.Timers;
using Rhino.Mocks;
using XpMan.LiftKata.FunctionalCode;

namespace XpMan.LiftKata.Tests
{
    public class TimerTestHelper
    {
        public void RaiseFakeTimerEvents(int numEvents, ITimer fakeTimer)
        {
            for (int eventCount = 1; eventCount <= numEvents; eventCount++)
            {
                fakeTimer.Raise(x => x.Elapsed += null, fakeTimer, new EventArgs() as ElapsedEventArgs);
            }
        }
    }
}