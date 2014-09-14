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

            this.RegisterAllNeededStapleMarketData(pastaParser);

            this.InstantiateAndSetupPricingAgentsForAllPasta(pastaParser);
        }

        private void InstantiateAndSetupPricingAgentsForAllPasta(PastaParser pastaParser)
        {
            // Instantiates pricing agents for all pastas
            foreach (var pastaName in pastaParser.Pastas)
            {
                var pastaPricingAgent = new PastaPricingAgent(pastaName);
                pastaPricingAgent.PastaPriceChanged += this.PastaPricingAgent_PastaPriceChanged;

                this.pastaAgents.Add(pastaName, pastaPricingAgent);

                var marketDataForThisPasta = new List<IStapleMarketData>();
                foreach (var stapleName in pastaParser.GetNeededStaplesFor(pastaName))
                {
                    marketDataForThisPasta.Add(this.marketDataProvider.GetStaple(stapleName));
                }

                pastaPricingAgent.SubscribeToMarketData(marketDataForThisPasta);
            }
        }

        private void RegisterAllNeededStapleMarketData(PastaParser pastaParser)
        {
            // subscribes to all the marketdata we need to price the pasta we have to support
            foreach (var marketDataName in pastaParser.StapleNames)
            {
                this.marketDataProvider.RegisterStaple(marketDataName);
            }
        }

        void PastaPricingAgent_PastaPriceChanged(object sender, PastaPriceChangedEventArgs e)
        {
            this.pastaPricerPublisher.Publish(e.PastaName, e.Price);
        }
    }
}
