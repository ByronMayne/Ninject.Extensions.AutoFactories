// -----------------------------| Notes |-----------------------------
// 1. Static constructor should be ignored and only instance constructor should generate factory method
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


namespace Widgets
{
    public partial class WidgetFactory : IWidgetFactory
    {
        public WidgetFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Widgets.Widget"/>
        /// </summary>
        public global::Widgets.Widget Create(string name)
        {
            global::Widgets.Widget __result = new global::Widgets.Widget(
             name);
            return __result;
        }
    }
}