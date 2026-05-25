// -----------------------------| Notes |-----------------------------
// 1. Create should have the name 'StringComparer'
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


namespace World
{
    public partial class PersonFactory : IPersonFactory
    {
        public PersonFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.Person"/>
        /// </summary>
        public global::World.Person Create(int? age, string? name)
        {
            global::World.Person __result = new global::World.Person(
             age,
             name);
            return __result;
        }
    }
}