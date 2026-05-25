// -----------------------------| Notes |-----------------------------
// 1. Nullable<T> for a struct (e.g. `Id?`) must not emit `Nullable<T>?` (CS0453).
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


namespace Sample
{
    public partial class WidgetSelectFactory : IWidgetSelectFactory
    {
        public WidgetSelectFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Sample.WidgetSelect"/>
        /// </summary>
        public global::Sample.WidgetSelect Create(global::Sample.Id? preselectedValue)
        {
            global::Sample.WidgetSelect __result = new global::Sample.WidgetSelect(
             preselectedValue);
            return __result;
        }
    }
}
