// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaParser.cs" company="No lock... no deadlock" product="Michonne">
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

    /// <summary>
    /// Parser to extract pasta names and their market data dependencies from a pasta configuration.
    /// </summary>
    public class PastaParser
    {
        private readonly IEnumerable<string> pastaConfiguration;

        private readonly List<string> pastaNames = new List<string>();
        private readonly List<string> marketDataNames = new List<string>();

        public PastaParser(IEnumerable<string> pastaConfiguration)
        {
            this.pastaConfiguration = pastaConfiguration;
            this.Parse();
        }

        public IEnumerable<string> Pastas
        {
            get
            {
                return this.pastaNames;
            }
        }

        public IEnumerable<string> MarketDataNames
        {
            get
            {
                return this.marketDataNames;
            }
        }

        private void Parse()
        {
            foreach (string pastaNameAndItsNeededMarketData in this.pastaConfiguration)
            {
                var splited = pastaNameAndItsNeededMarketData.Split('(');
                this.pastaNames.Add(splited[0]);

                var pastaNeededMarketData = splited[1].TrimEnd(')');
                var splittedMarketData = pastaNeededMarketData.Split('-');

                foreach (var marketData in splittedMarketData)
                {
                    if (!this.marketDataNames.Contains(marketData))
                    {
                        this.marketDataNames.Add(marketData);
                    }
                }
            }
        }
    }
}