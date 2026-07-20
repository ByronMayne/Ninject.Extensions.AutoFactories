// -----------------------------| Notes |-----------------------------
// 1. Two classes sharing a factory with same-named but different-typed [FromFactory] parameters should get unique fields
// -------------------------------------------------------------------
using AutoFactories;
using System.Collections.Generic;

namespace World
{
    public partial class Factory
    { }

    [AutoFactory(typeof(Factory))]
    public class Person
    {
        public Person(string name, [FromFactory] IEqualityComparer<string> service)
        { }
    }

    [AutoFactory(typeof(Factory))]
    public class Robot
    {
        public Robot(int id, [FromFactory] IComparer<string> service)
        { }
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
        private readonly global::System.Collections.Generic.IEqualityComparer<string> m_service;
        private readonly global::System.Collections.Generic.IComparer<string> m_service1;

        public Factory(
global::System.Collections.Generic.IEqualityComparer<string> service,
global::System.Collections.Generic.IComparer<string> service1)
        {
            m_service = service;
            m_service1 = service1;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.Person"/>
        /// </summary>
        public global::World.Person Create(string name)
        {
            global::World.Person __result = new global::World.Person(
             name,
             m_service);
            return __result;
        }


        /// <summary>
        /// Creates a new instance of  <see cref="World.Robot"/>
        /// </summary>
        public global::World.Robot Create(int id)
        {
            global::World.Robot __result = new global::World.Robot(
             id,
             m_service1);
            return __result;
        }
    }
}
global::World.Robot __result = new global::World.Robot(
                 id,
                 m_service1);
                return __result;
            }
        }
    }


    /// <summary>
    /// Creates a new instance of  <see cref="World.Robot"/>
    /// </summary>
    public global::World.Robot Create(int id)
        {
            global::World.Robot __result = new global::World.Robot(
             id,
             m_service1);
            return __result;
        }
    }
}