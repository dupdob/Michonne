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

    using Michonne.Implementation;
    using Michonne.Interfaces;

    public class PastaPricerEngine
    {
        private readonly IMarketDataProvider marketDataProvider;
        private readonly IPastaPricerPublisher pastaPricerPublisher;

        private readonly IUnitOfExecution unitOfExecution;

        private readonly IEnumerable<string> pastasConfiguration;

        private Dictionary<string, PastaPricingAgent> pastaAgents = new Dictionary<string, PastaPricingAgent>(); 

        public PastaPricerEngine(IUnitOfExecution unitOfExecution, IEnumerable<string> pastasConfiguration, IMarketDataProvider marketDataProvider, IPastaPricerPublisher pastaPricerPublisher)
        {
            this.unitOfExecution = unitOfExecution;
            this.pastasConfiguration = pastasConfiguration;
            this.marketDataProvider = marketDataProvider;
            this.pastaPricerPublisher = pastaPricerPublisher;
        }

        public void Start()
        {
            var pastaRecipeParser = new PastaRecipeParser(this.pastasConfiguration);

            this.RegisterAllNeededRawMaterialMarketData(pastaRecipeParser);

            this.InstantiateAndSetupPricingAgentsForAllPasta(pastaRecipeParser);
        }

        private void InstantiateAndSetupPricingAgentsForAllPasta(PastaRecipeParser pastaRecipeParser)
        {
            // Instantiates pricing agents for all pastas
            foreach (var pastaName in pastaRecipeParser.Pastas)
            {
                var sequencerForThisPasta = this.unitOfExecution.BuildSequencer();
                var pastaPricingAgent = new PastaPricingAgent(sequencerForThisPasta, pastaName);

                pastaPricingAgent.PastaPriceChanged += this.PastaPricingAgent_PastaPriceChanged;

                this.pastaAgents.Add(pastaName, pastaPricingAgent);

                var marketDataForThisPasta = new List<IRawMaterialMarketData>();
                foreach (var rawMaterialName in pastaRecipeParser.GetNeededRawMaterialsFor(pastaName))
                {
                    marketDataForThisPasta.Add(this.marketDataProvider.GetRawMaterial(rawMaterialName));
                }

                pastaPricingAgent.SubscribeToMarketData(marketDataForThisPasta);
            }
        }

        private void RegisterAllNeededRawMaterialMarketData(PastaRecipeParser pastaRecipeParser)
        {
            // subscribes to all the marketdata we need to price the pasta we have to support
            foreach (var marketDataName in pastaRecipeParser.RawMaterialNames)
            {
                this.marketDataProvider.RegisterRawMaterial(marketDataName);
            }
        }

        private void PastaPricingAgent_PastaPriceChanged(object sender, PastaPriceChangedEventArgs e)
        {
            this.pastaPricerPublisher.Publish(e.PastaName, e.Price);
        }
    }
}
