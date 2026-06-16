using AutoFactories.Types;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;

namespace AutoFactories
{
    internal static class TypeNames
    {
        public static MetadataTypeName ClassAttributeType { get; }
        public static MetadataTypeName FromFactoryAttributeType { get; }
        public static MetadataTypeName FactoryParamAttributeType { get; }
        public static AccessModifier AttributeAccessModifier { get; }

        static TypeNames()
        {
            AttributeAccessModifier = AccessModifier.Internal;
            ClassAttributeType = new MetadataTypeName(name: "AutoFactoryAttribute", @namespace:"AutoFactories", false, false);
            FromFactoryAttributeType = new MetadataTypeName(name: "FromFactoryAttribute", @namespace: "AutoFactories", false, false);
            FactoryParamAttributeType = new MetadataTypeName(name: "FactoryParamAttribute", @namespace: "AutoFactories", false, false);
        }
    }
}
