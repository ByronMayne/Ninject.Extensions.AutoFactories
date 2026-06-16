using AutoFactories.Diagnostics;
using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System.Collections;

namespace AutoFactories.Visitors
{

    internal class ParameterSyntaxVisitor : SyntaxVisitor<ParameterSyntax>
    {
        private readonly SemanticModel m_semanticModel;


        public string? Name { get; private set; }
        public MetadataTypeName Type { get; private set; }

        /// <summary>
        /// Gets whether the parameter has the <see cref="TypeNames.FromFactoryAttributeType"/> attribute.
        /// When true, this parameter will be resolved from the DI container.
        /// </summary>
        public bool HasFromFactoryAttribute { get; private set; }

        /// <summary>
        /// Gets whether the parameter has the <see cref="TypeNames.FactoryParamAttributeType"/> attribute.
        /// When true, this parameter must be passed to the factory method by the caller.
        /// </summary>
        public bool HasFactoryParamAttribute { get; private set; }

        /// <summary>
        /// Gets whether the parameter has either marker attribute.
        /// Kept for backwards compatibility.
        /// </summary>
        public bool HasMarkerAttribute => HasFromFactoryAttribute || HasFactoryParamAttribute;

        public AccessModifier Accessibility { get; private set; }

        public Location? AttributeLocation { get; private set; }

        public ConstructorDeclarationVisitor Constructor { get; }


        /// <summary>
        /// When we have a parameter that is generated from another source generator we can't
        /// actually resolve it. We can't get the type name or the fully qualified name. So we have to warn the user 
        /// </summary>
        public bool IsTypeResolved { get; private set; }

        public ParameterSyntaxVisitor(
            ConstructorDeclarationVisitor constructor,
            SemanticModel semanticModel)
        {
            Constructor = constructor;
            m_semanticModel = semanticModel;
        }


        protected override void Visit(ParameterSyntax syntax)
        {
            ITypeSymbol? typeSymbol = null;

            if (syntax.Type is not null)
            {
                typeSymbol = m_semanticModel.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;
            }

            MarkerAttributeResult markers = GetMarkerAttributes(syntax);
            Name = syntax.Identifier.Text;
            HasFromFactoryAttribute = markers.FromFactory is not null;
            HasFactoryParamAttribute = markers.FactoryParam is not null;
            AttributeLocation = markers.FromFactory?.GetLocation() ?? markers.FactoryParam?.GetLocation();

            if (typeSymbol is not null)
            {
                IsTypeResolved = true;
                Type = new MetadataTypeName(typeSymbol)
                {
                    IsNullable = syntax.Type is NullableTypeSyntax
                };
                Accessibility = AccessModifier.FromSymbol(typeSymbol);
            }
            else
            {
                IsTypeResolved = false;
                string typeName = $"{syntax.Type}";
                string @namespace = "";
                int splitIndex = typeName.LastIndexOf('.');

                if(splitIndex > 0)
                {
                    typeName = typeName.Substring(splitIndex + 1);
                    @namespace = typeName.Substring(0, splitIndex);
                }
                Type = new MetadataTypeName(typeName, @namespace, false, false);
                Accessibility = AccessModifier.Public; // We don't know what it is 

                AddDiagnostic(UnresolvedParameterTypeDiagnostic.Get(this));
            }

        }

        private MarkerAttributeResult GetMarkerAttributes(ParameterSyntax node)
        {
            AttributeSyntax? fromFactoryAttribute = null;
            AttributeSyntax? factoryParamAttribute = null;

            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (m_semanticModel.GetTypeInfo(attribute).Type is not ITypeSymbol typeSymbol)
                    {
                        continue;
                    }

                    string displayString = typeSymbol.ToDisplayString();

                    if (string.Equals(TypeNames.FromFactoryAttributeType.QualifiedName, displayString))
                    {
                        fromFactoryAttribute = attribute;
                    }
                    else if (string.Equals(TypeNames.FactoryParamAttributeType.QualifiedName, displayString))
                    {
                        factoryParamAttribute = attribute;
                    }
                }
            }

            return new MarkerAttributeResult(fromFactoryAttribute, factoryParamAttribute);
        }

        internal record MarkerAttributeResult(
            AttributeSyntax? FromFactory,
            AttributeSyntax? FactoryParam);
    }
}