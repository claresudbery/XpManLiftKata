using System;
using System.Collections.Generic;
using XpMan.LiftKata.FunctionalCode;

namespace XpMan.LiftKata.Tests
{
    internal class Lift : ILiftEngine, IDisposable, IObserver<int>
    {
        private readonly List<IObserver<int>> _observers = new List<IObserver<int>>();
        private readonly SimpleTimerLiftEngine _simpleTimerLiftEngine;

        public Lift(ITimer timerFake)
        {
            _simpleTimerLiftEngine = new SimpleTimerLiftEngine(timerFake);
            _simpleTimerLiftEngine.Subscribe(this);
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            _observers.Add(observer);

            return this;
        }

        public void Move(int floorToVisit)
        {
            _simpleTimerLiftEngine.Travel(0, floorToVisit);
        }

        public void Travel(int initialFloor, int destinationFloor)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnNext(int floorVisited)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(floorVisited);
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}