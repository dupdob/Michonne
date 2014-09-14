// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPricerEngine.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Collections.Generic;

    public class PastaPricerEngine
    {
        private readonly IMarketDataProvider marketDataProvider;
        private readonly IPastaPricerPublisher pastaPricerPublisher;
        private readonly IEnumerable<string> pastaConfiguration;

        private Dictionary<string, PastaPricingAgent> pastaAgents = new Dictionary<string, PastaPricingAgent>(); 

        public PastaPricerEngine(IEnumerable<string> pastaConfiguration, IMarketDataProvider marketDataProvider, IPastaPricerPublisher pastaPricerPublisher)
        {
            this.pastaConfiguration = pastaConfiguration;
            this.marketDataProvider = marketDataProvider;
            this.pastaPricerPublisher = pastaPricerPublisher;
        }

        public void Start()
        {
            var pastaParser = new PastaParser(this.pastaConfiguration);

            // Instantiates pricing agents for all pastas
            foreach (var pastaName in pastaParser.Pastas)
            {
                this.pastaAgents.Add(pastaName, new PastaPricingAgent(pastaName));

                // TODO: register for its needed marketdata
            }

            // subscribes to all the marketdata we need to price the pasta we have to support
            foreach (var marketDataName in pastaParser.MarketDataNames)
            {
                this.marketDataProvider.Register(marketDataName);
                this.marketDataProvider.Get(marketDataName).PriceChanged += this.PastaPricerEngine_PriceChanged;
            }
        }

        private void PastaPricerEngine_PriceChanged(object sender, PriceChangedEventArgs e)
        {
            this.pastaPricerPublisher.Publish(e.MarketDataName, e.Price);
        }
    }
}
