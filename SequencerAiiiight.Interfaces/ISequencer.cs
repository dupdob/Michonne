namespace SequencerAiiiight.Interfaces
{
    /// <summary>
    /// Allows to execute tasks asynchronously, but one by one and in the same order as they have been dispatched.
    /// That means that two tasks from the same dispatcher can be executed by two different threads, but NEVER in parallel. 
    /// Sequencer requirements are presented here: 
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// </summary>
    public interface ISequencer : IDispatcher
    {
    }
}