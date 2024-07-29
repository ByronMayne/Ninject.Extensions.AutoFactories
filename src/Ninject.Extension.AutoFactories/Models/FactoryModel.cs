using System.Diagnostics;

namespace Ninject.AutoFactories.Models
{
    [DebuggerDisplay("{Type}")]
    internal class FactoryModel
    {
        public MetadataTypeName Type { get; set; }
        public AccessModifier AccessModifier { get; set; }
        public MetadataTypeName InterfaceType { get; set; }
        public AccessModifier InterfaceAccessModifier { get; set; }
        public List<ProductModel> Products { get; set; }

        public FactoryModel(MetadataTypeName type)
        {
            Type = type;

            string interfaceName = $"I{type.TypeName}";
            if (!string.IsNullOrWhiteSpace(type.Namespace))
            {
                interfaceName = $"{type.Namespace}.{interfaceName}";
            }

            InterfaceType = new MetadataTypeName(interfaceName);
            AccessModifier = AccessModifier.Internal;
            InterfaceAccessModifier = AccessModifier.Internal;
            Products = [];
        }

        public static IEnumerable<FactoryModel> Group(IEnumerable<ProductModel> models)
        {
            Dictionary<string, FactoryModel> map = new Dictionary<string, FactoryModel>();

            foreach (ProductModel product in models)
            {
                if (!map.TryGetValue(product.FactoryType.FullName, out FactoryModel factory))
                {
                    factory = new FactoryModel(product.FactoryType);
                    map.Add(product.FactoryType.FullName, factory);
                }

                factory.Products.Add(product);
                if (product.FactoryAccessModifier.HasValue)
                {
                    factory.AccessModifier = product.FactoryAccessModifier.Value;
                }

                if (product.InterfaceAccessModifier.HasValue)
                {
                    factory.InterfaceAccessModifier = product.InterfaceAccessModifier.Value;
                }
            }

            return map.Values;
        }
    }
}
