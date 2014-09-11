
namespace Michonne
{
    using System;
    using Michonne.Interfaces;

    /// <summary>
    /// Dispatcher that keeps only the latest dispatched task, discarding the other dispatched tasks
    /// that couldn't have been executed.
    /// </summary>
    public sealed class BalkingDispatcher : IUnitOfExecution
    {
        private readonly object syncRoot = new object();
        private Action lastTask;
        private IUnitOfExecution rootDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalkingDispatcher"/> class.
        /// </summary>
        /// <param name="rootDispatcher">The root dispatcher.</param>
        public BalkingDispatcher(IUnitOfExecution rootDispatcher)
        {
            this.rootDispatcher = rootDispatcher;
        }

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <remarks>
        /// Depending on the concrete implementation of the dispatcher, the action will be
        /// executed asynchronouly (most likely) or synchronously (a few exceptions).
        /// </remarks>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.lastTask = action;
            }

            this.rootDispatcher.Dispatch(this.ExecuteLastTask);
        }

        private void ExecuteLastTask()
        {
            Action action = null;
            lock (this.syncRoot)
            {
                action = this.lastTask;
                this.lastTask = null;
            }

            if (action != null)
            {
                action();
            }
        }
    }
}