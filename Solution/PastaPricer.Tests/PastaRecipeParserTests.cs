﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaRecipeParserTests.cs" company="No lock... no deadlock" product="Michonne">
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
namespace PastaPricer.Tests
{
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class PastaRecipeParserTests
    {
        [Test]
        public void Should_extract_pasta_names_from_configuration()
        {
            var pastasConfiguration = new[]
                                      {
                                          "gnocchi(eggs-potatoes-flour)",
                                          "spaghetti(eggs-flour)",
                                          "organic spaghetti(organic eggs-flour)",
                                          "spinach farfalle(eggs-flour-spinach)",
                                          "tagliatelle(eggs-flour)",
                                      };

            var pastaParser = new PastaRecipeParser(pastasConfiguration);

            Check.That(pastaParser.Pastas).ContainsExactly("gnocchi", "spaghetti", "organic spaghetti", "spinach farfalle", "tagliatelle");
        }

        [Test]
        public void Should_extract_RawMaterialNames_from_configuration()
        {
            var pastasConfiguration = new[]
                                      {
                                          "gnocchi(eggs-potatoes-flour)",
                                          "spaghetti(eggs-flour)",
                                          "organic spaghetti(organic eggs-flour)",
                                          "spinach farfalle(eggs-flour-spinach)",
                                          "tagliatelle(eggs-flour)",
                                      };

            var pastaParser = new PastaRecipeParser(pastasConfiguration);
            Check.That(pastaParser.RawMaterialNames).ContainsExactly("eggs", "potatoes", "flour", "organic eggs", "spinach");
        }

        [Test]
        public void Should_provide_NeededRawMaterials_for_every_pasta()
        {
            var pastasConfiguration = new[]
                                      {
                                          "gnocchi(eggs-potatoes-flour)",
                                          "spaghetti(eggs-flour)",
                                          "organic spaghetti(organic eggs-flour)",
                                          "spinach farfalle(eggs-flour-spinach)",
                                          "tagliatelle(eggs-flour)",
                                      };

            var pastaParser = new PastaRecipeParser(pastasConfiguration);

            Check.That(pastaParser.GetNeededRawMaterialsFor("gnocchi")).ContainsExactly("eggs", "potatoes", "flour");
            Check.That(pastaParser.GetNeededRawMaterialsFor("spinach farfalle")).ContainsExactly("eggs", "flour", "spinach");
        }
    }
}
