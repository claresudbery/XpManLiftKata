using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace XpMan.LiftKata.Tests
{
    // Based on the work Martin and I did as a pair, when this was run as an exercise during code freeze at LateRooms
    [TestFixture]
    public class StartingFromScratchTests
    {
        List<int> _floorsVisited = new List<int>();

        [Test]
        public void FloorsShouldBeBroadcastInOrderWhenUpwardsRequestIsMade()
        {
            var lift = new LateRoomsLift(0);
            lift.FloorVisitAction = VisitFloor;

            lift.Visit(2);

            Assert.That(_floorsVisited.Count, Is.EqualTo(2));
            Assert.That(_floorsVisited[0], Is.EqualTo(1));
            Assert.That(_floorsVisited[1], Is.EqualTo(2));
        }

        [Test]
        public void FloorsShouldBeBroadcastInOrderWhenDownwardsRequestIsMade()
        {
            var lift = new LateRoomsLift(3);
            lift.FloorVisitAction = VisitFloor;

            lift.Visit(0);

            Assert.That(_floorsVisited.Count, Is.EqualTo(3));
            Assert.That(_floorsVisited[0], Is.EqualTo(2));
            Assert.That(_floorsVisited[1], Is.EqualTo(1));
            Assert.That(_floorsVisited[2], Is.EqualTo(0));
        }

        public void VisitFloor(int floor)
        {
            _floorsVisited.Add(floor);
        }
    }

    public class LateRoomsLift
    {
        private int _currentFloor;
        public Action<int> FloorVisitAction { get; set; }

        public LateRoomsLift(int initialFloor)
        {
            _currentFloor = initialFloor;
        }

        public void Visit(int destinationFloor)
        {
            int movement = destinationFloor > _currentFloor ? 1 : -1;

            while (_currentFloor != destinationFloor)
            {
                _currentFloor += movement;
                FloorVisitAction(_currentFloor);
            }
        }
    }
}