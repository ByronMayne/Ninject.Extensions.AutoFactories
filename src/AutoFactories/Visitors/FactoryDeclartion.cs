using AutoFactories.Types;
using HandlebarsDotNet;
using Ninject.AutoFactories;
using System;
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
        {
            // The shared ("untyped") factory hoists every [FromFactory] parameter into a private
            // field. Because a field is emitted per hoisted parameter and the generated Create
            // method body references that field by name, two problems can occur across the
            // aggregated constructors:
            //   1. Parameters of the same type produce duplicate fields.
            //   2. Parameters with the same name but different types produce conflicting fields.
            // To always emit compilable code we collapse the hoisted parameters down to a single
            // field per distinct type, give each field a unique name, and remap every constructor
            // parameter to the shared field name so the method body stays in sync.
            List<ParameterViewModel> hoistedFields = new List<ParameterViewModel>();
            Dictionary<string, string> fieldNamesByType = new Dictionary<string, string>(StringComparer.Ordinal);
            HashSet<string> usedFieldNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (ParameterSyntaxVisitor parameter in declaration.Parameters)
            {
                ParameterViewModel model = ParameterViewModel.Map(parameter);
                string typeKey = model.Type.QualifiedName;

                if (fieldNamesByType.ContainsKey(typeKey))
                {
                    // A field for this type already exists; reuse it and drop the duplicate.
                    continue;
                }

                string fieldName = model.Name;
                int suffix = 1;
                while (!usedFieldNames.Add(fieldName))
                {
                    fieldName = $"{model.Name}{suffix++}";
                }

                model.Name = fieldName;
                fieldNamesByType[typeKey] = fieldName;
                hoistedFields.Add(model);
            }

            List<FactoryMethodViewModel> methods = declaration.Classes
                .SelectMany(c => c.Constructors)
                .Where(c => !c.IsStatic)
                .Select(FactoryMethodViewModel.Map)
                .ToList();

            // Point each hoisted method parameter at the shared field so the method body reference
            // (m_{Name}) resolves to the merged field regardless of the original parameter name.
            foreach (FactoryMethodViewModel method in methods)
            {
                foreach (ParameterViewModel parameter in method.Parameters)
                {
                    if (!parameter.IsRequired &&
                        fieldNamesByType.TryGetValue(parameter.Type.QualifiedName, out string fieldName))
                    {
                        parameter.Name = fieldName;
                    }
                }
            }

            return new FactoryViewModel()
            {
                Type = declaration.Type,
                Usings = declaration.Usings.ToList(),
                ImplementationAccessModifier = declaration.ImplementationAccessModifier,
                InterfaceAccessModifier = declaration.InterfaceAccessModifier,
                Parameters = hoistedFields,
                Methods = methods
            };
        }
    }
}
