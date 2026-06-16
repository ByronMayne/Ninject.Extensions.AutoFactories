// -----------------------------| Notes |-----------------------------
// 1. [FactoryParam] marks parameters that should be passed to the factory method.
// 2. Parameters without [FactoryParam] should come from DI.
// 3. 'name' has [FactoryParam] so it appears in Create() method signature.
// 4. 'comparer' does NOT have [FactoryParam] so it becomes a DI-injected field.
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


namespace Products
{
    public partial class ProductFactory : IProductFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string?> m_comparer;

        public ProductFactory(
global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            m_comparer = comparer;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Products.Product"/>
        /// </summary>
        public global::Products.Product Create(string name)
        {
            global::Products.Product __result = new global::Products.Product(
             name,
             m_comparer);
            return __result;
        }
    }
}