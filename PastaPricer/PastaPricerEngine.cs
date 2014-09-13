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

    public class PastaPricerEngine
    {
        private readonly IMarketDataProvider marketDataProvider;
        private readonly IPastaPricerPublisher pastaPricerPublisher;

        public PastaPricerEngine(IMarketDataProvider marketDataProvider, IPastaPricerPublisher pastaPricerPublisher)
        {
            this.marketDataProvider = marketDataProvider;
            this.pastaPricerPublisher = pastaPricerPublisher;
        }

        public void Start()
        {
            this.marketDataProvider.Get("eggPrice").PriceChanged += this.PastaPricerEngine_PriceChanged;
        }

        private void PastaPricerEngine_PriceChanged(object sender, EventArgs e)
        {
            this.pastaPricerPublisher.Publish(string.Empty, 0);
        }
    }
}
