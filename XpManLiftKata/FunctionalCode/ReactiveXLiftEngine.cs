using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace XpMan.LiftKata.FunctionalCode
{
    public class ReactiveXLiftEngine : IObservable<int>, IDisposable, ILiftEngine
    {
        private readonly List<IObserver<int>> _observers = new List<IObserver<int>>();
        private readonly IScheduler _scheduler;
        private IDisposable _liftEngineSubscription = null;
        private int _destinationFloor;
        private bool _moving = false;

        public ReactiveXLiftEngine(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            _observers.Add(observer);
            return this;
        }

        public void Dispose()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public void Travel(int initialFloor, int destinationFloor)
        {
            _destinationFloor = destinationFloor;

            if (_moving)
            {
                Stop();
            }

            _moving = true;

            _liftEngineSubscription = Observable.Generate
                (
                // Starting value is this (first part of the for statement)
                    initialFloor,

                    // Keep going as long as this is true (second part of the for statement)
                    floor => initialFloor < destinationFloor
                        ? initialFloor < destinationFloor
                        : initialFloor > destinationFloor,

                    // This tells us how to get to the next iterator (third part of the for statement)
                    floor => initialFloor < destinationFloor
                        ? floor + 1
                        : floor - 1,

                    // On each iteration, output the following result (body of the for statement)
                    floor => floor,

                    // Specify the iteration time between each result
                    floor => TimeSpan.FromMilliseconds(TimeConstants.FloorInterval),

                    // Specify which scheduler should be used to schedule events
                    _scheduler
                )
                .Subscribe
                (
                // This function will be executed whenever an event (ie an iteration in the above loop) is generated
                // The input to this function will be the output from the fourth step above ("body of the for statement")
                    ArrivedAtFloor
                );
        }

        private void ArrivedAtFloor(int floor)
        {
            NotifyObserversOfCurrentStatus(floor);

            if (floor == _destinationFloor)
            {
                Stop();
            }
        }

        private void NotifyObserversOfCurrentStatus(int floor)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(floor);
            }
        }

        private void Stop()
        {
            _moving = false;
            _liftEngineSubscription.Dispose();
        }
    }
}