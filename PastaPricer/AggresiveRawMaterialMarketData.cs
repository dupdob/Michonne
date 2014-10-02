namespace PastaPricer
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides market data as events for a given raw material.
    /// </summary>
    /// <remarks>This type is thread-safe</remarks>
    public class AggresiveRawMaterialMarketData : IRawMaterialMarketData
    {
        private static Random seed = new Random(1);
        
        private readonly int timerPeriodInMsec;

        private readonly int aggressionFactor;

        private Timer timer;
        private long stopped = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggresiveRawMaterialMarketData"/> class.
        /// </summary>
        /// <param name="rawMaterialName">
        /// Name of the raw material.
        /// </param>
        /// <param name="timerPeriodInMsec">
        /// The timer period in milliseconds.
        /// </param>
        /// <param name="aggressionFactor">
        /// The aggression Factor.
        /// </param>
        public AggresiveRawMaterialMarketData(string rawMaterialName, int timerPeriodInMsec = 9, int aggressionFactor = 300)
        {
            this.RawMaterialName = rawMaterialName;
            this.timerPeriodInMsec = timerPeriodInMsec;
            this.aggressionFactor = aggressionFactor;
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
                        for (var i = 0; i < this.aggressionFactor; i++)
                        {
                            decimal randomPrice = seed.Next(1, 20) / 10m;
                            this.RaisePrice(randomPrice);
                        }
                    }
                    else
                    {
                        // the last notification should always be 0.
                        this.RaisePrice(0m);
                        
                        this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                        this.timer.Dispose();
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
        }

        private void RaisePrice(decimal price)
        {
            if (this.PriceChanged != null)
            {
                this.PriceChanged(this, new RawMaterialPriceChangedEventArgs(this.RawMaterialName, price));
            }
        }
    }
}