namespace Michonne.Interfaces
{
    using System;

    public interface IUnitOfExecutionsFactory
    {
        /// <summary>
        /// Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IUnitOfExecution" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        IUnitOfExecution GetDedicatedThread();

        /// <summary>
        /// Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IUnitOfExecution" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        IUnitOfExecution GetPool();

        /// <summary>
        /// Build a <see cref="ISequencer"/> wrapping an <see cref="IUnitOfExecution"/>.
        /// </summary>
        /// <param name="execution"><see cref="IUnitOfExecution"/> that will carry out sequenced task.</param>
        /// <returns>A <see cref="ISequencer"/> instance.</returns>
        ISequencer GetSequence(IUnitOfExecution execution);
    }
}