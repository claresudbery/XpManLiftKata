using System;

namespace XpMan.LiftKata.FunctionalCode
{
    public interface ILiftEngine
    {
        IDisposable Subscribe(IObserver<int> observer);
        void Travel(int initialFloor, int destinationFloor);
    }
}