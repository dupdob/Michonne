namespace SequencerAiiiight
{
    using System;
    using System.Threading;

    using SequencerAiiiight.Interfaces;

    /// <summary>
    /// Allow to dispatch actions/tasks for asynchronous execution through the classical .NET thread pool.
    /// </summary>
    public class DotNetThreadPoolDispatcher : IDispatcher
    {
        #region Public Methods and Operators

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <remarks>
        ///     With this dispatcher, the action will be executed asynchronouly.
        /// </remarks>
        /// <param name="action">The action to be executed asynchronously.</param>
        public void Dispatch(Action action)
        {
            ThreadPool.QueueUserWorkItem((_) => action());
        }

        #endregion
    }
}