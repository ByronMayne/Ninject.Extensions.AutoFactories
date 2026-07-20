// -----------------------------| Notes |-----------------------------
// 1. Two classes sharing a factory with the same [FromFactory] type should emit a single merged field
// -------------------------------------------------------------------
using AutoFactories;
using System.Collections.Generic;

namespace World
{
    public partial class Factory
    {}

    [AutoFactory(typeof(Factory))]
    public class Person
    {
        public Person(string name, [FromFactory] IEqualityComparer<string> comparer)
        {}
    }

    [AutoFactory(typeof(Factory))]
    public class Robot
    {
        public Robot(int id, [FromFactory] IEqualityComparer<string> comparer)
        {}
    }
}
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


namespace World
{
    public partial class Factory : IFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string> m_comparer;

        public Factory(
global::System.Collections.Generic.IEqualityComparer<string> comparer)
        {
            m_comparer = comparer;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.Person"/>
        /// </summary>
        public global::World.Person Create(string name)
        {
            global::World.Person __result = new global::World.Person(
             name,
             m_comparer);
            return __result;
        }


        /// <summary>
        /// Creates a new instance of  <see cref="World.Robot"/>
        /// </summary>
        public global::World.Robot Create(int id)
        {
            global::World.Robot __result = new global::World.Robot(
             id,
             m_comparer);
            return __result;
        }
    }
}