using Michonne.Interfaces;

namespace Michonne.Sources.Tests
{
    using System;
    using System.Collections.Concurrent;

    internal class StepperUnit : IUnitOfExecution
    {
        private ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public IUnitOfExecutionsFactory UnitOfExecutionsFactory { get; }

        public void Dispatch(Action action)
        {
            this.actions.Enqueue(action);
        }

        public void Step()
        {
            Action result;
            if (this.actions.TryDequeue(out result))
            {
                result.Invoke();
            }
        }
    }
}