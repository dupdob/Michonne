// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StepperUnit.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StepperUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Michonne.Interfaces;

namespace Michonne.Sources.Tests
{
#if !NET20 && !NET30
    using System;
#endif
    using System.Collections.Concurrent;

    internal class StepperUnit : IExecutor
    {
        private ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public IExecutorFactory ExecutorFactory { get; }

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

        internal void Step(int v)
        {
            Action result;
            while (this.actions.TryDequeue(out result) && v > 0)
            {
                result.Invoke();
                v--;
            }
        }
    }
}