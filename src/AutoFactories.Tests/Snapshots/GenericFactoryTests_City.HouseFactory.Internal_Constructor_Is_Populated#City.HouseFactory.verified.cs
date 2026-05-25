// -----------------------------| Notes |-----------------------------
// 1. The constructor is internal but should still generate a factory method
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


namespace City
{
    public partial class HouseFactory : IHouseFactory
    {
        public HouseFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="City.House"/>
        /// </summary>
        public global::City.House Create(string address, int? unitNumber)
        {
            global::City.House __result = new global::City.House(
             address,
             unitNumber);
            return __result;
        }
    }
}