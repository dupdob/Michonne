using System;
using Michonne.Interfaces;

namespace Michonne.Tests
{
    internal class SynchroCall : IUnitOfExecution
    {
        public int DoneTasks;

        #region IUnitOfExecution Members

        public void Dispatch(Action action)
        {
            action();
            this.DoneTasks++;
        }

        #endregion
    }
}