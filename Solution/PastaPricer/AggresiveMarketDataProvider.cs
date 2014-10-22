namespace PastaPricer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides <see cref="RawMaterialMarketData"/> instances for registered raw material names.
    /// </summary>
    public class AggresiveMarketDataProvider : IMarketDataProvider
    {
        private readonly int timerPeriodInMsec;

        private readonly int aggressionFactor;

        private readonly Dictionary<string, AggresiveRawMaterialMarketData> rawMaterialMarketDatas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggresiveMarketDataProvider"/> class.
        /// </summary>
        /// <param name="timerPeriodInMsec">
        /// The timer Period In milliseconds.
        /// </param>
        /// <param name="aggressionFactor"> Number of notifications to generate each period.
        /// </param>
        public AggresiveMarketDataProvider(int timerPeriodInMsec, int aggressionFactor)
        {
            this.timerPeriodInMsec = timerPeriodInMsec;
            this.aggressionFactor = aggressionFactor;
            this.rawMaterialMarketDatas = new Dictionary<string, AggresiveRawMaterialMarketData>();
        }

        /// <summary>   a
        /// Registers the specified raw material, so that it can be started and retrieved afterwards.
        /// </summary>
        /// <param name="rawMaterialNameToRegister">The raw material name to register.</param>
        public void RegisterRawMaterial(string rawMaterialNameToRegister)
        {
            // TODO: make it thread-safe
            if (!this.rawMaterialMarketDatas.ContainsKey(rawMaterialNameToRegister))
            {
                this.rawMaterialMarketDatas.Add(rawMaterialNameToRegister, new AggresiveRawMaterialMarketData(rawMaterialNameToRegister, timerPeriodInMsec: this.timerPeriodInMsec, aggressionFactor: this.aggressionFactor));
            }
        }

        /// <summary>
        /// Starts all the registered <see cref="RawMaterialMarketData" /> instances.
        /// </summary>
        public void Start()
        {
            // TODO: make it thread-safe
            foreach (var rawMaterialMarketData in this.rawMaterialMarketDatas.Values)
            {
                rawMaterialMarketData.Start();
            }
        }

        /// <summary>
        /// Gets the <see cref="RawMaterialMarketData" /> instance corresponding to this raw material name.
        /// </summary>
        /// <param name="rawMaterialName">Name of the raw material.</param>
        /// <returns>
        /// The <see cref="RawMaterialMarketData" /> instance corresponding to this raw material name.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When the raw material is not registered yet to receive market data.</exception>
        public IRawMaterialMarketData GetRawMaterial(string rawMaterialName)
        {
            // TODO: make it thread-safe
            AggresiveRawMaterialMarketData rawMaterialMarketData;
            
            if (!this.rawMaterialMarketDatas.TryGetValue(rawMaterialName, out rawMaterialMarketData))
            {
                throw new InvalidOperationException(string.Format("RawMaterial with name '{0}' is not registered yet for market data. Call the RegisterRawMaterial method for it before you get it.", rawMaterialName));
            }

            return rawMaterialMarketData;
        }

        /// <summary>
        /// Stops all the registered <see cref="RawMaterialMarketData" /> instances.
        /// </summary>
        public void Stop()
        {
            foreach (var rawMaterialMarketData in this.rawMaterialMarketDatas.Values)
            {
                rawMaterialMarketData.Stop();
            }
        }
    }
}