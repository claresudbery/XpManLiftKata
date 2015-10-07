using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace XpMan.LiftKata.FunctionalCode
{
    public class SimpleTimerLiftEngine : ILiftEngine, IDisposable
    {
        private static ITimer _myTimer;
        private readonly List<IObserver<int>> _observers = new List<IObserver<int>>();
        private int _currentFloor = 0;
        private int _destinationFloor = 0;
        private bool _goingUp = true;
        private bool _moving;

        public SimpleTimerLiftEngine(ITimer timer)
        {
            _myTimer = timer;

            _myTimer.Start(TimeConstants.FloorInterval);

            _myTimer.Elapsed += OnNext;
            _myTimer.AutoReset = true;
            _myTimer.Enabled = true;

            _moving = false;
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            _observers.Add(observer);
            return this;
        }

        public void Travel(int initialFloor, int destinationFloor)
        {
            _goingUp = initialFloor <= destinationFloor;
            _currentFloor = _goingUp ? initialFloor - 1 : initialFloor + 1;
            _destinationFloor = destinationFloor;
            _moving = true;
        }

        private void OnNext(Object source, ElapsedEventArgs e)
        {
            if (_moving)
            {
                string eventTime = "[no time]";
                if (e != null)
                {
                    eventTime = string.Format(
                        "{0:HH:mm:ss.fff}",
                        e.SignalTime);
                }

                _currentFloor = _goingUp ? _currentFloor + 1 : _currentFloor - 1;

                Debug.WriteLine(
                    "The next-floor event was for floor {0}, and was raised at {1}",
                    _currentFloor,
                    eventTime);

                foreach (var observer in _observers)
                {
                    observer.OnNext(_currentFloor);
                }

                if (_currentFloor == _destinationFloor)
                {
                    Stop();
                }
            }
        }

        private void Stop()
        {
            _moving = false;
        }

        public void Dispose()
        {
            _myTimer.Stop();
            _myTimer.Dispose();
        }
    }
}