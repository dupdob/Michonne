namespace SequencerAiiiight.Interfaces
{
    using System;

    /// <summary>
    /// Allow to dispatch actions/tasks for execution. 
    /// A dispatcher may be whether asynchronous (more likely) or synchronous.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <remarks>
        ///     Depending on the concrete implementation of the dispatcher, the action will be 
        ///     executed asynchronouly (most likely) or synchronously (a few exceptions).</remarks>
        /// <param name="action">The action to be executed</param>
        void Dispatch(Action action);
    }
}