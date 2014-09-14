// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="StapleMarketData.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Provides market data as events for a given staple.
    /// </summary>
    /// <remarks>This type is thread-safe</remarks>
    public class StapleMarketData
    {
        private readonly int timerPeriodInMsec;

        private Timer timer;
        private long stopped = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StapleMarketData"/> class.
        /// </summary>
        /// <param name="stapleName">Name of the staple.</param>
        /// <param name="timerPeriodInMsec">The timer period in milliseconds.</param>
        public StapleMarketData(string stapleName, int timerPeriodInMsec = 9)
        {
            this.StapleName = stapleName;
            this.timerPeriodInMsec = timerPeriodInMsec;
        }
        
        /// <summary>
        /// Occurs when the price of this staple changed.
        /// </summary>
        public event EventHandler<StaplePriceChangedEventArgs> StaplePriceChanged;

        /// <summary>
        /// Gets the name of the Staple corresponding to this <see cref="StapleMarketData"/> instance.
        /// </summary>
        /// <value>
        /// The name of the Staple corresponding to this <see cref="StapleMarketData"/> instance.
        /// </value>
        public string StapleName { get; private set; }

        /// <summary>
        /// Starts to receive market data (and thus to raise events) for this staple.
        /// </summary>
        public void Start()
        {
            this.timer = new Timer(
                                        delegate
                                        {
                                            var hasStopped = Interlocked.CompareExchange(ref this.stopped, 1, 1);
                                            if (hasStopped != 1)
                                            {
                                                this.RaiseRandomPrice();
                                            }
                                        }, 
                                        null, 
                                        0, 
                                        this.timerPeriodInMsec);
        }

        /// <summary>
        /// Stops to receive market data (and thus to raise events) for this staple.
        /// </summary>
        public void Stop()
        {
            // Tries to stop the action being done by the timer ASAP.
            Interlocked.Exchange(ref this.stopped, 1);

            // and the timer also.
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.timer.Dispose();
        }

        private void RaiseRandomPrice()
        {
            if (this.StaplePriceChanged != null)
            {
                this.StaplePriceChanged(this, new StaplePriceChangedEventArgs(this.StapleName, 0));
            }
        }
    }
}