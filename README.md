# ElevatorKata01
elevator kata - first attempt (see http://blog.milesburton.com/2013/03/28/elevator-kata-mind-bending-pairing-exercise/)

- An elevator responds to calls containing a source floor and direction
- An elevator delivers passengers to requested floors
- Elevator calls are queued not necessarily FIFO
- You may validate passenger floor requests
- you may implement current floor monitor
- you may implement direction arrow
- you may implement doors (opening and closing)
- you may implement DING!
- there can be more than one elevator
- ?? Max number of lift occupants?

Related links:

A blog post wot I rote about making the tests readable (I'll write another one soon with more detail about the actual kata): http://engineering.laterooms.com/making-tests-readable/

ReactiveX installer:
https://www.microsoft.com/en-us/download/details.aspx?id=30708
 
ReactiveX tutorials (I’m working my way through “Curing the asynchronous blues with the Reactive Extensions for .NET” – see “Tutorials and Articles” at the bottom of the page):
https://msdn.microsoft.com/en-gb/data/gg577611

Using schedulers for testing:
http://www.introtorx.com/content/v1.0.10621.0/16_TestingRx.html
http://haacked.com/archive/2014/03/10/master-time-with-reactive-extensions/

http://www.quora.com/Is-there-any-public-elevator-scheduling-algorithm-standard

http://www.quora.com/Why-are-virtually-all-elevator-algorithms-so-inefficient

Paternoster: https://www.youtube.com/watch?v=Ro3Fc_yG3p0

SCHEDULERS AND REACTIVEX
I need to add some notes here about how the TestScheduler class works, because it's been confusing the hell out of me:
- If you want to schedule events at a particular time, you need to use the Schedule method
- The AdvanceBy and AdvanceTo methods use Ticks, which are tiny (one tick is equal to 100 nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond)
- When you call Start, all actions scheduled so far are run at once
- When you call AdvanceBy, all actions scheduled in the specified time period are run at once
- When you pass a scheduler to an Observable.Generate call, this has the effect of scheduling the relevant events
	-- so a subsequent call to Start or AdvanceBy will run the scheduled events up to the specified point
	-- but they won't be run until Start or AdvanceBy or AdvanceTo are called
	
A note on the test helpers:
The following methods are used to make the tests easier to write and easier to read:
	LiftMakeStartAt
	LiftExpectToLeaveFrom
	LiftExpectToVisit
	LiftExpectToStopAt
	LiftMakeUpwardsRequestFrom
	LiftMakeDownwardsRequestFrom
	LiftMakeRequestToMoveTo
	StartTest
	Mark
	VerifyAllMarkers
The reason these methods exist is that otherwise, you have to keep careful track of...
	exactly what events you expect to occur
	exactly how many events you expect to occur
	exactly what time you expect each event to occur (so that you can insert lift moves and calls in the correct places)
By using the new methods, you don't have to count events or keep track of time - the methods do that for you.
	Note that most of the methods are not actually making anything happen
	- they are just making a note of what we expect to happen, 
	and allowing us to make a note of the time and the order in which these events should happen.
	Thus there are two types of method:
		Those prefixed LiftExpectTo, which note expectations and at what time / in what order each expected event will happen
		Those prefixed LiftMake, which actually call methods on the lift itself
	However you can't have one without the other, because the LiftMake methods use the scheduler
		...and in order to use the scheduler, you need to know at what time you want the lift to be called
	Also, you need the LiftExpectTo methods to have noted the order of events so that you can verify expectations later on.
Then you use the Mark method to make a note of which events you want to verify, 
	and call VerifyAllMarkers to make the relevant assertions at the end of the test.
	The fact that you explicitly call LiftExpectToVisit, LiftExpectToStop etc means that there is a clear visible record of what you expect the lift to do.
	This has significantly reduced the quantity of pain in my head whilst writing these tests!

Note that the basic algorithm being used at this point is pretty simple:
	- When the lift is idle, the first person to make a request defines the current direction of lift movement
	- The lift will continue moving in the current direction until all satisfiable requests have been satisfied 
		(eg if moving upwards, only requests from people who are currently higher than the lift, and who want to move upwards, will be serviced))
	- Then the lift will change direction and satisfy all satisfiable requests in the new direction
	- When the lift is idle, it will return to the ground floor
	- It is possible that in a tall building with people making short journeys between floors, it might be more efficient to satisfy some slightly-lower upwards requests before switching to downwards requests, but for now we are not taking that into account
		It seems reasonable to assume that in most buildings, most journeys are either to or from the ground floor anyway.
		But a more sophisticated future version of the software might have analytics which examine the most-frequently-made journeys and change behaviour in response?
		...or just at any point in time, examine all pending requests and calculate which journeys would be most efficient 
			(although the definition of 'efficient' could presumably change depending on whose needs you are trying to satisfy)
			(and there is probably a danger that some poor person would get left stranded on the top floor because fetching them would never represent the most efficient use of the resource)
	
Multiple lifts - thoughts: 	
	NB: We need to consider lift capacities.
	NB: I've decided to ignore energy efficiency and focus purely on getting everyone from A to B as quickly as possible.
	NB: The speed with which a lift can be expected respond to a request is calculated like this:
		For every destination in the lift's list of destinations, 
			it is assumed that the lift will spend 5 seconds at that destination
				even though people getting out of lifts might be slightly quicker than people getting into lifts,
				because the lift doesn't have to wait for them to make a move request
		It is also assumed that the last destination in whatever the direction the lift is currently moving
			is the last destination it will visit in that direction
			(even though this may in fact change when someone new enters the lift and asks it to move somewhere else)
		Time-to-destination is recalculated every time the lift is asked how long it would take 
			(because the data will be constantly changing)
		?? Will requests be taken away from lifts and reassigned to other players in response to changed circumstances?
			Would it be better to just wait until each lift is passing a floor in a particular direction	
				and then see whether they should stop for whoever happens to be waiting?
				It would be interesting to compare this model where lift users are told in advance which lift they should use
				- which would be most effective?
				The problem with this approach would be that requests would not be assigned to a lift
					until the lift is already moving
					which might mean the lift never starts moving in the first place, because it is never given a request to service?
					Maybe the lifts are given specific jobs to do if they are lying idle, 
						but once all lifts are in motion, no new jobs are assigned until a lift is about to pass or arrive at the floor in question.
						Also, although the general rule is that new jobs are always given to idle lifts,
							there may be times when lifts already in flight are closer than lifts still on the ground floor
							so we can't just assume that all new requests are given to idle lifts
							then again, we could just send the idle lift off in that direction anyway
							in case the other one gets held up
							and then just wait to see who gets there first
							... in which case, we might have the situ where four requests come in from floors 20, 21, 22 and 23
							and all four lifts are sent in the same direction
							but in fact just one of those lifts is able to service all four requests
							and in the meantime some other requests come in from much lower down
							at which point some or all of those lifts should be diverted
							so maybe each lift is never given a destination, just a direction
							and then it is told when to stop on an ad hoc basis?
								although if this approach is followed, then as soon as someone enters the lift and tells it where to go,
								the lift will have to have a concrete destination
								in which case should the lift itself be responsible for maintaining at least this subset of destinations
								or will that info be maintained by the lift manager?
							this should be part of a separate strategy
							and the two strategies could be compared

Multiple lifts - Tests which might need writing:
	When people make 'move' requests that are opposite direction to their initial 'call' request,
		the lift will still honour their request once it has finished going in the other direction.
	If one lift is already in flight and a new request comes in which is 
		in the same direction 
		and on the same route the lift is already moving in
		and from a floor the lift has not yet passed
		and no other lift is available which can get there more quickly
		then the new request will be given to the lift already in flight
		but actually...
			you can ignore that big list of criteria
			they will be used by the lift to calculate how quickly it can arrive at the caller's floor
			but all the manager cares about is the answer to the question, "how quickly can you reach this caller?"
		so in fact, the rule is just
			when a new request comes in, it is given to the lift which can reach the caller first
			this may or may not be a lift which is already in flight
			see above for a whole load of ideas about not allocating the lift to the caller until a lift is passing the caller
			but that would be such a big logic change that I reckon it should be implemented as an entirely separate strategy
			and then the two can be compared

Other tests which might need writing:
If the lift runs out of upwards requests 
	... and starts processing downwards requests...
	... but the next downwards request is coming from a higher floor...
	... and while the lift is moving up to that floor, an upwards request comes in which lies between the lift's current location and the floor it is moving to...
	... what should happen??
		The problem is, that when the lift arrives at the location it has been called to, the caller might ask it to go to an even higher location than the one it was aiming for. 
			...meaning that it would have to overshoot.
			...and someone else might make an even higher upwards request while it processes the new one.
		One solution is that we say No, we are now processing downwards requests and we will ignore all upwards requests until we are done with our downwards requests.
			This is what is currently implemented, I think, because we say if direction is not Down then we are moving up (otherwise we are moving down)
			The problem with this is that users will see the lift is coming towards them, but ignoring them and going straight past.
		The alternative is that if an upwards request comes in which we are able to service on our way to the downwards request, we just cancel the downwards strategy and consider ourselves to be moving upwards again.
			For now I'm leaving this with the status quo - ie the new upwards request is ignored until all downwards requests are serviced
			(I've written a test to check that this is indeed the case:
			Given_lift_has_finished_up_requests_and_the_next_downwards_request_comes_from_higher_up_when_a_new_up_request_comes_in_while_travelling_to_the_down_request_then_the_up_request_will_be_ignored_until_later)
Simpler lift-stopping algorithm:
	When the lift arrives at a floor it is supposed to stop at, either because it is picking someone up from there or because it is dropping someone off (or both),
	it will wait a certain amount of time (during which we can assume it has opened and closed its doors) and then move on.
	Any new Move requests made during this time are simply added to the queue.
		! This means that the current functionality needs to take into account that when someone makes a Move request and the lift is not moving, this may be because it is waiting for a Wait() timer to complete.
		or in other words, when a Wait timer completes, if the lift has already started moving again, it can quietly die.
		! But what if a Wait timer is started, then a Move request is made, then the lift arrives at a new floor and a new Wait timer is kicked off, and then the PREVIOUS Wait timer expires?
		Presumably all existing Wait timers should be killed whenever a new one is kicked off.
More sophisticated lift-stopping algorithm:
	We assume that whenever the lift stops, it opens its doors, and then closes them when it is ready to move on.
	When lift reaches floor where person is supposed to be waiting, it will move on after either	
		a) somebody tells the lift where they want to go
		b) a certain amount of time has elapsed
	When lift reaches floor where person is supposed to be exiting, it will move on after either
		a) somebody exits the lift
		b) a certain amount of time has elapsed
	When lift reaches floor where person is supposed to be waiting AND person is supposed to be exiting, 
	it will ignore the exiting requirement and prioritise the boarding requirement, ie it will move on after either	
		a) somebody tells the lift where they want to go
		b) a certain amount of time has elapsed
At some point in the future, we might want to be a bit more sophisticated about what "Wait" means:
	- Don't just wait five seconds for something to happen:
	- If you've reached the end of an upwards or downwards series of actions, just wait until someone has exited the lift, then change direction
		- needs to have awareness of people entering / leaving lift
		- might need to know the difference of arriving somewhere in order to deliver someone, and arriving somewhere in order to pick someone up
If somebody calls the lift while it is stopped on a floor, it will start moving again
	This test is written (When_lift_is_called_while_already_stopped_somewhere_else_it_will_respond_to_the_new_request), but...
		At some point we will need to add something in which understands that first it needs to make sure that it has opened the doors and given people a chance to enter
		Also maybe we need to detect whether somebody is literally in the process of entering / leaving the lift before closing the doors??
		I think this would lead us to split this test into two versions:
			If somebody calls the lift while it is stopped on a floor because somebody called it there, it will start moving again
			If somebody calls the lift while it is stopped on a floor because somebody has just exited the lift, it will start moving again
			
Possible technologies for a UI: 
	recommended by Braithers:
		Ruby + Sinatra
		Angular
		Node.js
	recommended by Google:
		Xamarin (based on my own quick Googling re what I could use to generate an app for Android and iOS, but using languages / technologies I already know (ie C# and Visual Studio))
			Details here: http://nnish.com/2013/06/12/how-i-built-an-android-app-in-c-visual-studio-in-less-than-24hrs/
			
Multiple lifts:
	Make start:
		Both lifts: _millisecondsSinceTestStarted = 1000
	Manager gets upwards request from 3
		_millisecondsTakenByMostRecentEvent = 1500
		schedule a "MakeUpwardsRequest" event for 1500 ms
	Lift A leaves the ground floor
		Lift A: _millisecondsSinceTestStarted gets 1500 added on (= 2500)
		Lift A: _millisecondsTakenByMostRecentEvent = 1000
	Manager gets downwards request from 2
		_millisecondsTakenByMostRecentEvent = 1500
		schedule a "MakeDownwardsRequest" event for 3000 ms
	Lift B leaves the ground floor
		Lift B: _millisecondsSinceTestStarted gets 1500 added on (= 2500)
		Lift B: _millisecondsTakenByMostRecentEvent = 1000
		
	1.5: make upwards request from 3
	2.5: Lift A leaves ground floor
	3.5: Lift A visits 1st floor
	4.5: Lift A visits 2nd floor
	5.5: Lift A stops at 3rd floor
	
	3: Downwards request from 5
	4: Lift B leaves ground floor
	5: Lift B visits 1st floor
	6: Lift B visits 2nd floor
	7: Lift B visits 3rd floor
	8: Lift B visits 4th floor
	9: Lift B stops at 5th floor

TO DO:
	ManagerMakeDownwardsRequestFrom: Now has two "expected" values: 
		shouldBeActedUponImmediately and expectedLiftName - so really these should somehow go into some kind of Expect method,
		rather than a Make method?
		
QUS FOR XPMAN:
	Are my tests actually acceptance tests? 
		Should I have simpler unit tests which simply test the logic of individual lift methods?