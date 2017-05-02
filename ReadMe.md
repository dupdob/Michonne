![Michonne](https://github.com/dupdob/michonne/blob/master/michonne.png?raw=true)

__When you have locks within your applicative code... you will have deadlock. Period.__ The only question you may ask is "when?"
Same thing with API allowing you to wait without indicating any kind of timeout (e.g. [TPL](http://msdn.microsoft.com/en-us/library/dd235635.aspx)). Too bad... and too easy to shoot yourself in the foot...

But how to protect your code from race conditions without using lock?!? Easy answer: let Michonne help you.

What's Michonne?
==============

__*"No lock... no deadlock!"*__  Michonne is a library that allows you __to erradicate deadlocks within your applicative code__ by providing you composable behaviors for tasks/lambdas execution (i.e. dispatcher, sequencer, balking dispatcher, etc), in replacement of locks and raw TPL primitives that will lead you to deadlocks.

Related resources
---------------
+ __[Posts serie about the Sequencer pattern](http://dupdob.wordpress.com/tag/sequencer/)__ Posts written by the father of this lib (__Cyrille DUPUYDAUBY__) where he explains the origin and the caracteristics of the sequencer pattern.
+ __[Basses latences, hauts debits...](https://www.youtube.com/watch?v=SHptUbGxXMU)__ A french talk made at __DEVOXX FR__ 2014, where Cyrille and I talked about reactive programming, and explained the sequencer pattern, the notion of conflation etc.
