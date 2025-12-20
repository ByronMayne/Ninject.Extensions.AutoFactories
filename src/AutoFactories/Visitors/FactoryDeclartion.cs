using AutoFactories.Types;
using HandlebarsDotNet;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;
using AutoFactories.Templating;
using AutoFactories.Models;

namespace AutoFactories.Visitors
{

    internal class FactoryDeclaration
    {
        public MetadataTypeName Type { get; }
        public AccessModifier ImplementationAccessModifier { get; }
        public AccessModifier InterfaceAccessModifier { get; }
        public IReadOnlyList<string> Usings { get; }
        public IReadOnlyList<ClassDeclarationVisitor> Classes { get; }
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters { get; }


        private FactoryDeclaration(MetadataTypeName type, IEnumerable<ClassDeclarationVisitor> classes)
        {
            Type = type;
            Classes = classes.ToList();
            Parameters = Classes
                .SelectMany(c => c.Constructors)
                .Where(c => !c.IsPrivate)
                .Where(c => !c.IsStatic)
                .SelectMany(c => c.Parameters)
                .Where(p => p.HasMarkerAttribute)
                .ToList();

            Usings = classes.SelectMany(c => c.Usings)
                .Distinct()
                .OrderBy(s => s.StartsWith("System") ? 0 : 1)
                .ThenBy(s => s)
                .ToList();

            InterfaceAccessModifier = AccessModifier.MostRestrictive(
                Classes
                .Select(c => c.InterfaceAccessModifier)
                .ToArray());

            ImplementationAccessModifier = AccessModifier.MostRestrictive(
                Classes
                .Select(c => c.FactoryAccessModifier)
                .ToArray());
        }

        public static IEnumerable<FactoryDeclaration> Create(IEnumerable<ClassDeclarationVisitor> classes)
        {
            foreach (IGrouping<MetadataTypeName, ClassDeclarationVisitor> grouping in classes.GroupBy(v => v.FactoryType))
            {
                yield return new FactoryDeclaration(grouping.Key, grouping);
            }
        }


        public static FactoryViewModel Map(FactoryDeclaration declaration)
            => new FactoryViewModel()
            {
                Type = declaration.Type,
                Usings = declaration.Usings.ToList(),
                ImplementationAccessModifier = declaration.ImplementationAccessModifier,
                InterfaceAccessModifier = declaration.InterfaceAccessModifier,
                Parameters = declaration.Parameters
                    .Select(ParameterViewModel.Map)
                    .ToList(),
                Methods = declaration.Classes
                    .SelectMany(c => c.Constructors)
                    .Where(c => !c.IsStatic)
                    .Select(FactoryMethodViewModel.Map)
                    .ToList()
            };
    }
}
