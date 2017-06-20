// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StepperUnit.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StepperUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Michonne.Implementation
{
#if !NET20 && !NET30
    using System;
#if !NET35
    using System.Collections.Concurrent;
#endif
#endif

    using Michonne.Interfaces;

    public class StepperUnit : IExecutor
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

        public void Step(int v)
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