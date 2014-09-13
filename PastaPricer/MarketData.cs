// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MarketData.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
namespace PastaPricer
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides market data as event.
    /// </summary>
    /// <remarks>This type is thread-safe</remarks>
    public class MarketData
    {
        private Timer timer;
        private long stopped = 0;

        private readonly int timerPeriodInMsec;

        public event EventHandler PriceChanged;

        public MarketData(int timerPeriodInMsec = 9)
        {
            this.timerPeriodInMsec = timerPeriodInMsec;
        }

        public void Start()
        {
            this.timer = new Timer(delegate
            {
                var hasStopped = Interlocked.CompareExchange(ref this.stopped, 1, 1);
                if (hasStopped != 1)
                {
                    this.RaiseRandomPrice();
                }
            }, null, 0, this.timerPeriodInMsec);
        }

        private void RaiseRandomPrice()
        {
            if (this.PriceChanged != null)
            {
                this.PriceChanged(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            // Tries to stop the action being done by the timer ASAP.
            Interlocked.Exchange(ref this.stopped, 1);
            // and the timer also.
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.timer.Dispose();
        }
    }
}