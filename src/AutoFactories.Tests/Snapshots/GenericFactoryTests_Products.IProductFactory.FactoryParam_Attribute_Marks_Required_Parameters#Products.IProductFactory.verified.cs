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
    public partial interface IProductFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Products.Product"/>
        /// </summary>
        global::Products.Product Create(string name);
    }
}
