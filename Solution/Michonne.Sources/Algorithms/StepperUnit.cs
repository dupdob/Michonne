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
    /// <inheritdoc />
    /// <summary>
    /// Step by step IExecutor implementation
    /// </summary>
    public class StepperUnit : IExecutor
    {
        private ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Underlying executor.
        /// </summary>
        public IExecutorFactory ExecutorFactory { get; }
        
        /// <inheritdoc />
        public void Dispatch(Action action)
        {
            this.actions.Enqueue(action);
        }

        /// <summary>
        /// Explicitely requests execution a single task.
        /// </summary>
        public void Step()
        {
            if (this.actions.TryDequeue(out var result))
            {
                result.Invoke();
            }
        }

        /// <summary>
        /// Explicitely requests execution of <paramref name="v"/> tasks.
        /// </summary>
        /// <param name="v">Number of tasks to be executed.</param>
        public void Step(int v)
        {
            while (this.actions.TryDequeue(out var result) && v > 0)
            {
                result.Invoke();
                v--;
            }
        }
    }
}