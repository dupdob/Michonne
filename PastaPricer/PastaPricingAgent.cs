// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPricingAgent.cs" company="No lock... no deadlock" product="Michonne">
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
    /// <summary>
    /// Computes prices for a given pasta.
    /// </summary>
    public class PastaPricingAgent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPricingAgent"/> class.
        /// </summary>
        /// <param name="pastaName">Name of the pasta.</param>
        public PastaPricingAgent(string pastaName)
        {
            this.PastaName = pastaName;
        }

        /// <summary>
        /// Gets the name of the pasta to price.
        /// </summary>
        /// <value>
        /// The name of the pasta to price.
        /// </value>
        public string PastaName { get; private set; }
    }
}