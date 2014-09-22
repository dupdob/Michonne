﻿namespace PastaPricer
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides market data as events for a given raw material.
    /// </summary>
    /// <remarks>This type is thread-safe</remarks>
    public class AggresiveRawMaterialMarketData : IRawMaterialMarketData
    {
        private readonly int timerPeriodInMsec;

        private Timer timer;
        private long stopped = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggresiveRawMaterialMarketData"/> class.
        /// </summary>
        /// <param name="rawMaterialName">Name of the raw material.</param>
        /// <param name="timerPeriodInMsec">The timer period in milliseconds.</param>
        public AggresiveRawMaterialMarketData(string rawMaterialName, int timerPeriodInMsec = 9)
        {
            this.RawMaterialName = rawMaterialName;
            this.timerPeriodInMsec = timerPeriodInMsec;
        }

        /// <summary>
        /// Occurs when the price of this raw material changed.
        /// </summary>
        public event EventHandler<RawMaterialPriceChangedEventArgs> PriceChanged;

        /// <summary>
        /// Gets the name of the raw material corresponding to this <see cref="AggresiveRawMaterialMarketData" /> instance.
        /// </summary>
        /// <value>
        /// The name of the raw material corresponding to this <see cref="AggresiveRawMaterialMarketData" /> instance.
        /// </value>
        public string RawMaterialName { get; private set; }

        /// <summary>
        /// Starts to receive market data (and thus to raise events) for this raw material.
        /// </summary>
        public void Start()
        {
            this.timer = new Timer(
                delegate
                {
                    var hasStopped = Interlocked.CompareExchange(ref this.stopped, 1, 1);
                    if (hasStopped != 1)
                    {
                        for (var i = 0; i < 1000; i++)
                        {
                            this.RaiseRandomPrice();
                        }
                    }
                }, 
                null, 
                0, 
                this.timerPeriodInMsec);
        }

        /// <summary>
        /// Stops to receive market data (and thus to raise events) for this raw material.
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
            if (this.PriceChanged != null)
            {
                this.PriceChanged(this, new RawMaterialPriceChangedEventArgs(this.RawMaterialName, 0));
            }
        }
    }
}