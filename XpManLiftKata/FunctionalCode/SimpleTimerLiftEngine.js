var _currentFloor = 0;
var _destinationFloor = 0;
var _goingUp = true;
var _moving = false;
var _onNext = noSubscribers;
var MillisecondsBetweenFloors = 1000;

var myVar = setInterval(myTimer, MillisecondsBetweenFloors);

function noSubscribers(floor) {
    console.log("There are no subscribers yet, but we are on floor " + floor);
}

function Subscribe(onNext) {
    _onNext = onNext;
}

function Travel(initialFloor, destinationFloor) {
    _goingUp = initialFloor <= destinationFloor;
    _currentFloor = _goingUp ? initialFloor - 1 : initialFloor + 1;
    _destinationFloor = destinationFloor;
    _moving = true;
}

function myTimer() {
    var d = new Date();
    
    if (_moving) {
        var eventTime = d.toLocaleTimeString();

        _currentFloor = _goingUp ? _currentFloor + 1 : _currentFloor - 1;

        console.log("The next-floor event was for floor " + _currentFloor + ", and was raised at " + eventTime);

        _onNext(_currentFloor);

        if (_currentFloor === _destinationFloor)
        {
            Stop();
        }
    }
}

function Stop() {
    _moving = false;
}