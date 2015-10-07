using System;
using System.Collections.Generic;
using System.Timers;
using XpMan.LiftKata.FunctionalCode;
using NUnit.Framework;
using Rhino.Mocks;

namespace XpMan.LiftKata.Tests
{
    [TestFixture]
    public class SimpleTimerLiftEngineTests : IObserver<int>
    {
        private readonly List<int> _floorsVisited = new List<int>();
        private readonly ITimer _timerFake = MockRepository.GenerateStub<ITimer>();
        
        [Test]
        public void When_Lift_Moves_From_One_Floor_To_The_Next_Floor_Up_Then_Both_Floors_Will_Be_Visited_In_Order()
        {
            // Arrange
            _floorsVisited.Clear();
            SimpleTimerLiftEngine simpleTimerLiftEngine = new SimpleTimerLiftEngine(_timerFake);
            simpleTimerLiftEngine.Subscribe(this);

            // Act
            simpleTimerLiftEngine.Travel(1, 2);

            RaiseFakeTimerEvents(2);

            // Assert
            Assert.That(_floorsVisited.Count, Is.EqualTo(2), "Expected two floor-visiting events");
            Assert.That(_floorsVisited[0], Is.EqualTo(1), "Expected to visit floor 1 first");
            Assert.That(_floorsVisited[1], Is.EqualTo(2), "Expected to visit floor 2 second");
        }

        [Test]
        public void When_Lift_Moves_From_One_Floor_To_The_Next_Floor_Down_Then_Both_Floors_Will_Be_Visited_In_Order()
        {
            // Arrange
            _floorsVisited.Clear();
            SimpleTimerLiftEngine simpleTimerLiftEngine = new SimpleTimerLiftEngine(_timerFake);
            simpleTimerLiftEngine.Subscribe(this);

            // Act
            simpleTimerLiftEngine.Travel(2, 1);

            RaiseFakeTimerEvents(2);

            // Assert
            Assert.That(_floorsVisited.Count, Is.EqualTo(2), "Expected two floor-visiting events");
            Assert.That(_floorsVisited[0], Is.EqualTo(2), "Expected to visit floor 2 first");
            Assert.That(_floorsVisited[1], Is.EqualTo(1), "Expected to visit floor 1 second");
        }

        [Test]
        public void When_Lift_Moves_From_One_Floor_To_Another_Floor_Then_All_Floors_Will_Be_Visited_In_Order()
        {
            // Arrange
            _floorsVisited.Clear();
            SimpleTimerLiftEngine simpleTimerLiftEngine = new SimpleTimerLiftEngine(_timerFake);
            simpleTimerLiftEngine.Subscribe(this);

            // Act
            simpleTimerLiftEngine.Travel(7, 2);

            RaiseFakeTimerEvents(6);

            // Assert
            Assert.That(_floorsVisited.Count, Is.EqualTo(6), "Expected six floor-visiting events");
            Assert.That(_floorsVisited[0], Is.EqualTo(7), "Expected to visit floor 7 first");
            Assert.That(_floorsVisited[1], Is.EqualTo(6), "Expected to visit floor 6 second");
            Assert.That(_floorsVisited[2], Is.EqualTo(5), "Expected to visit floor 5 third");
            Assert.That(_floorsVisited[3], Is.EqualTo(4), "Expected to visit floor 4 fourth");
            Assert.That(_floorsVisited[4], Is.EqualTo(3), "Expected to visit floor 3 fifth");
            Assert.That(_floorsVisited[5], Is.EqualTo(2), "Expected to visit floor 2 sixth");
        }

        [Test]
        public void When_Lift_Gets_A_New_Request_Mid_Flight_Then_All_Events_Are_Reset_Even_If_It_Makes_No_Sense()
        {
            // Arrange
            _floorsVisited.Clear();
            SimpleTimerLiftEngine simpleTimerLiftEngine = new SimpleTimerLiftEngine(_timerFake);
            simpleTimerLiftEngine.Subscribe(this);

            // Act
            simpleTimerLiftEngine.Travel(7, 2);

            RaiseFakeTimerEvents(3);

            simpleTimerLiftEngine.Travel(3, 5);

            RaiseFakeTimerEvents(3);

            // Assert
            Assert.That(_floorsVisited.Count, Is.EqualTo(6), "Expected six floor-visiting events");

            Assert.That(_floorsVisited[0], Is.EqualTo(7), "Expected to visit floor 7 first");
            Assert.That(_floorsVisited[1], Is.EqualTo(6), "Expected to visit floor 6 second");
            Assert.That(_floorsVisited[2], Is.EqualTo(5), "Expected to visit floor 5 third");

            Assert.That(_floorsVisited[3], Is.EqualTo(3), "Expected to visit floor 3 fourth");
            Assert.That(_floorsVisited[4], Is.EqualTo(4), "Expected to visit floor 4 fifth");
            Assert.That(_floorsVisited[5], Is.EqualTo(5), "Expected to visit floor 5 sixth");
        }

        private void RaiseFakeTimerEvents(int numEvents)
        {
            for (int eventCount = 1; eventCount <= numEvents; eventCount++)
            {
                _timerFake.Raise(x => x.Elapsed += null, _timerFake, new EventArgs() as ElapsedEventArgs);
            }
        }

        public void OnNext(int floorVisited)
        {
            _floorsVisited.Add(floorVisited);
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