namespace PastaPricer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RecipeHelper
    {
        private static readonly string[] FlourKind = { "flour", "premium flour", "organic flour", "whole flour" };

        private static readonly string[] EggKind = { "eggs", "organic eggs" };

        private static readonly string[] FlavorKind = { "tomato", "potatoes", "spinach" };

        private static readonly string[] Sizes = { "small", "medium", "large" };

        private static readonly string[] Packaging = { "plastic", "cardboard", "paper" };

        /// <summary>
        /// The parse raw material role.
        /// </summary>
        /// <param name="rawMaterialName">
        /// The raw material name.
        /// </param>
        /// <returns>
        /// The <see cref="RawMaterialRole"/>.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// When the string is not a known ingredient.
        /// </exception>
        public static RawMaterialRole ParseRawMaterialRole(string rawMaterialName)
        {
            if (FlourKind.Contains(rawMaterialName))
            {
                return RawMaterialRole.Flour;
            }

            if (EggKind.Contains(rawMaterialName))
            {
                return RawMaterialRole.Egg;
            }

            if (FlavorKind.Contains(rawMaterialName))
            {
                return RawMaterialRole.Flavor;
            }

            if (Sizes.Contains(rawMaterialName))
            {
                return RawMaterialRole.Size;
            }

            if (Packaging.Contains(rawMaterialName))
            {
                return RawMaterialRole.Packaging;
            }
            
            throw new ApplicationException(rawMaterialName + " unknown ingredient");
        }

        public static IEnumerable<string> GenerateConfigurations()
        {
            var result = new List<string>();
            var count = 0;
            foreach (var flour in FlourKind)
            {
                foreach (var egg in EggKind)
                {
                    foreach (var flavor in FlavorKind)
                    {
                        foreach (var size in Sizes)
                        {
                            foreach (var s in Packaging)
                            {
                                result.Add(string.Format("pasta #{0}({1}-{2}-{3}-{4}-{5})", count, flour, egg, flavor, size, s));
                                count++;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}