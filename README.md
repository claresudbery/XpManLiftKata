This repo contains some simple starter code to get people started on the lift kata.

This is the problem you will be trying to solve:

- There is only one lift
- Your lift should respond to calls containing a source floor and direction
- Your lift should deliver passengers to requested floors
- Your lift will not know where its passengers are going until they enter the lift and ask for a specific floor
- You should aim to respond to each request as quickly as possible
- If a passenger calls the lift and specifies that they are travelling up, but then asks for a lower floor, 
	their request will be ignored if it affects the lift's ability to respond to previous requests
- You may implement current floor monitor - ie the lift informs observers of its current location and direction

Related links:

The original concept is described here (it's worth noting that I've narrowed the scope due to the limited time available):
http://blog.milesburton.com/2013/03/28/elevator-kata-mind-bending-pairing-exercise/)

A blog post I wrote about my own attempts at this kata - specifically how I made my tests more readable 
(I'll probably be writing more here about the actual kata in the coming weeks / months): 
http://engineering.laterooms.com/making-tests-readable/

ReactiveX installer:
https://www.microsoft.com/en-us/download/details.aspx?id=30708
 
ReactiveX tutorials (I recommend “Curing the asynchronous blues with the Reactive Extensions for .NET” – see “Tutorials and Articles” at the bottom of the page - although some of it is out of date):
https://msdn.microsoft.com/en-gb/data/gg577611

Using schedulers for testing:
http://www.introtorx.com/content/v1.0.10621.0/16_TestingRx.html
http://haacked.com/archive/2014/03/10/master-time-with-reactive-extensions/

http://www.quora.com/Is-there-any-public-elevator-scheduling-algorithm-standard

http://www.quora.com/Why-are-virtually-all-elevator-algorithms-so-inefficient

Paternoster: https://www.youtube.com/watch?v=Ro3Fc_yG3p0

SCHEDULERS AND REACTIVEX
I'm adding some notes here about how the TestScheduler class works, because it confused the hell out of me:
- If you want to schedule events at a particular time, you need to use the Schedule method
- The AdvanceBy and AdvanceTo methods use Ticks, which are tiny (one tick is equal to 100 nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond)
- When you call Start, all actions scheduled so far are run at once
- When you call AdvanceBy, all actions scheduled in the specified time period are run at once
- When you pass a scheduler to an Observable.Generate call, this has the effect of scheduling the relevant events
	-- so a subsequent call to Start or AdvanceBy will run the scheduled events up to the specified point
	-- but they won't be run until Start or AdvanceBy or AdvanceTo are called