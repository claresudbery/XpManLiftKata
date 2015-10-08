using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using XpMan.LiftKata.FunctionalCode;

namespace XpMan.LiftKata.Tests
{
    [TestFixture]
    class SampleTests : IObserver<int>
    {
        private readonly List<int> _floorsVisited = new List<int>();
        private readonly ITimer _timerFake = MockRepository.GenerateStub<ITimer>();
        private readonly TimerTestHelper _timerTestHelper = new TimerTestHelper();

        [Test]
        public void When_person_in_lift_enters_a_higher_floor_number_then_lift_goes_to_that_floor()
        {
            // Arrange
            _floorsVisited.Clear();
            var theLift = new Lift(_timerFake);
            theLift.Subscribe(this);

            // Act
            theLift.Move(3);

            _timerTestHelper.RaiseFakeTimerEvents(1, _timerFake);

            // Assert
            Assert.That(_floorsVisited[0], Is.EqualTo(0), "should start on ground floor");
            Assert.That(_floorsVisited[3], Is.EqualTo(3), "fourth lift event should be 3rd floor");
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
