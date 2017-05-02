// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaRecipeParser.cs" company="No lock... no deadlock" product="Michonne">
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
    public class PastaRecipeParser
    {
        private readonly IEnumerable<string> pastaConfiguration;

        private readonly List<string> pastaNames = new List<string>();
        private readonly List<string> rawMaterialNames = new List<string>();

        private readonly Dictionary<string, IEnumerable<string>> perPastaNeededRawMaterials = new Dictionary<string, IEnumerable<string>>();

        public PastaRecipeParser(IEnumerable<string> pastaConfiguration)
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

        public IEnumerable<string> RawMaterialNames
        {
            get
            {
                return this.rawMaterialNames;
            }
        }

        /// <summary>
        /// Gets the needed raw materials for a given pasta name.
        /// </summary>
        /// <param name="pastaName">Name of the pasta.</param>
        /// <returns>The needed raw materials for this pasta.</returns>
        public IEnumerable<string> GetNeededRawMaterialsFor(string pastaName)
        {
            return this.perPastaNeededRawMaterials[pastaName];
        }

        private void Parse()
        {
            foreach (string pastaLine in this.pastaConfiguration)
            {
                var splited = pastaLine.Split('(');
                var pastaName = splited[0];
                this.pastaNames.Add(pastaName);

                var pastaNeededRawMaterials = splited[1].TrimEnd(')');
                var requestedRawMaterialsForThisPasta = pastaNeededRawMaterials.Split('-');

                this.perPastaNeededRawMaterials[pastaName] = requestedRawMaterialsForThisPasta;

                // Stores the list of all requested raw materials 
                foreach (var rawMaterialName in requestedRawMaterialsForThisPasta)
                {
                    if (!this.rawMaterialNames.Contains(rawMaterialName))
                    {
                        this.rawMaterialNames.Add(rawMaterialName);
                    }
                }
            }
        }
    }
}