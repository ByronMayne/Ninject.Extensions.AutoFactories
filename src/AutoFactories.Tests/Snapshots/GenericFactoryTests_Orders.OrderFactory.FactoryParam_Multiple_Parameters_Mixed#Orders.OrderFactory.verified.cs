// -----------------------------| Notes |-----------------------------
// 1. Multiple parameters with mixed [FactoryParam] usage.
// 2. 'name' and 'quantity' have [FactoryParam] - they appear in Create().
// 3. 'logger' and 'validator' do NOT have [FactoryParam] - they come from DI.
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


namespace Orders
{
    public partial class OrderFactory : IOrderFactory
    {
        private readonly global::Orders.ILogger m_logger;        private readonly global::Orders.IValidator m_validator;

        public OrderFactory(
global::Orders.ILogger logger,
global::Orders.IValidator validator)
        {
            m_logger = logger;
            m_validator = validator;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Orders.Order"/>
        /// </summary>
        public global::Orders.Order Create(string name, int quantity)
        {
            global::Orders.Order __result = new global::Orders.Order(
             name,
             m_logger,
             quantity,
             m_validator);
            return __result;
        }
    }
}