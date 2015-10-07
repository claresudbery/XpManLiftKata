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

            MakeTimerKeepFiringForTheLifeOfThisObject();

            _moving = false;
        }

        private void MakeTimerKeepFiringForTheLifeOfThisObject()
        {
            _myTimer.Start(TimeConstants.FloorInterval);

            _myTimer.Elapsed += OnNext;
            _myTimer.AutoReset = true;
            _myTimer.Enabled = true;
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            _observers.Add(observer);

            return this;
        }

        public void Travel(int initialFloor, int destinationFloor)
        {
            _goingUp = AreWeGoingUp(initialFloor, destinationFloor);

            _currentFloor = TheFloorBefore(initialFloor);

            _destinationFloor = destinationFloor;
            _moving = true;
        }

        private int TheFloorBefore(int initialFloor)
        {
            return _goingUp ? initialFloor - 1 : initialFloor + 1;
        }

        private bool AreWeGoingUp(int initialFloor, int destinationFloor)
        {
            return initialFloor <= destinationFloor;
        }

        private void OnNext(Object source, ElapsedEventArgs elapsedEventArgs)
        {
            if (_moving)
            {
                IncrementCurrentFloor();

                LogEventForDebug(elapsedEventArgs);

                InformObserversThatFloorHasBeenReached();

                StopMovingIfWeHaveReachedOurDestination();
            }
        }

        private void StopMovingIfWeHaveReachedOurDestination()
        {
            if (_currentFloor == _destinationFloor)
            {
                Stop();
            }
        }

        private void InformObserversThatFloorHasBeenReached()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(_currentFloor);
            }
        }

        private void LogEventForDebug(ElapsedEventArgs elapsedEventArgs)
        {
            string eventTime = "[no time]";
            if (elapsedEventArgs != null)
            {
                eventTime = string.Format(
                    "{0:HH:mm:ss.fff}",
                    elapsedEventArgs.SignalTime);
            }

            Debug.WriteLine(
                "The next-floor event was for floor {0}, and was raised at {1}",
                _currentFloor,
                eventTime);
        }

        private void IncrementCurrentFloor()
        {
            _currentFloor = _goingUp ? _currentFloor + 1 : _currentFloor - 1;
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