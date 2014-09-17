namespace Michonne.Implementation
{
    using Michonne.Interfaces;


    /// <summary>
    /// Static class hosting the sequencer factory API.
    /// </summary>
    public static class SequencerFactory
    {
        /// <summary>
        /// The build sequencer.
        /// </summary>
        /// <param name="executor">
        /// The executor.
        /// </param>
        /// <returns>
        /// The <see cref="ISequencer"/>.
        /// </returns>
        public static ISequencer BuildSequencer(this IUnitOfExecution executor)
        {
            return new Sequencer(executor);
        }
    }
}