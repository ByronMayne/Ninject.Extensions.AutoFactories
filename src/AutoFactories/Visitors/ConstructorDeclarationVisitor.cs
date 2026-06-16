using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFactories.Visitors
{
    internal class ConstructorDeclarationVisitor : SyntaxVisitor<ConstructorDeclarationSyntax>
    {
        private readonly bool m_isAnalyzer;
        private readonly SemanticModel m_semanticModel;
        private readonly List<ParameterSyntaxVisitor> m_parameters;

        /// <summary>
        /// Gets if it's a static constructor or not 
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets if the constructor is public or not
        /// </summary>
        public bool IsPrivate { get; private set; }

        /// <summary>
        /// Gets the type that is going to be created
        /// </summary>
        public MetadataTypeName Type { get; }

        /// <summary>
        /// Gets the type that the constructor will make 
        /// </summary>
        public MetadataTypeName ReturnType { get; }

        /// <summary>
        /// Gets the access modifier that the constructor will need to have
        /// </summary>
        public AccessModifier Accessibility { get; private set; }

        /// <summary>
        /// Gets the collection of parameters for the constructor
        /// </summary>
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters => m_parameters;


        /// <summary>
        /// Gets the class tha the constructor is defined within
        /// </summary>
        public ClassDeclarationVisitor Class { get; }

        /// <summary>
        /// Gets the location of the constructor declaration
        /// </summary>
        public Location? Location { get; private set; }

        /// <summary>
        /// Gets whether this constructor uses <see cref="TypeNames.FactoryParamAttributeType"/> mode
        /// to determine which parameters are required in the factory method.
        /// <para>
        /// There are two mutually exclusive modes for marking constructor parameters:
        /// <list type="bullet">
        ///   <item>
        ///     <see cref="TypeNames.FromFactoryAttributeType"/> mode: parameters marked with
        ///     the attribute are resolved from the DI container; all others are required in the factory method.
        ///   </item>
        ///   <item>
        ///     <see cref="TypeNames.FactoryParamAttributeType"/> mode: parameters marked with
        ///     the attribute are required in the factory method; all others are resolved from DI.
        ///   </item>
        /// </list>
        /// If neither attribute is present on any parameter, all parameters are required.
        /// Mixing both attributes in the same constructor produces a diagnostic error.
        /// </para>
        /// </summary>
        public bool UsesFactoryParamMode { get; private set; }

        public ConstructorDeclarationVisitor(
            bool isAnalyzer, 
            ClassDeclarationVisitor classVisitor, 
            INamedTypeSymbol type,
            INamedTypeSymbol returnType, 
            SemanticModel semanticModel)
        {
            m_isAnalyzer = isAnalyzer;
            m_semanticModel = semanticModel;
            m_parameters = new List<ParameterSyntaxVisitor>();
            Type = new MetadataTypeName(type);
            ReturnType = new MetadataTypeName(returnType);
            Accessibility = AccessModifier.FromSymbol(returnType); // Default access is the return type
            Class = classVisitor;
        }

        protected override void Visit(ConstructorDeclarationSyntax syntax)
        {
            IsStatic = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
            IsPrivate = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword));
            Location = syntax.GetLocation();

            VisitParameters(syntax.ParameterList);

            // Returns back the most restrictive permissions 
            // for all the parameters an return type. This should be public or internal
            Accessibility = AccessModifier.MostRestrictive([
                 Accessibility,
                 ..m_parameters.Select(p => p.Accessibility)]);
            
            ValidateParameterAttributes();
        }

        private void ValidateParameterAttributes()
        {
            bool hasFromFactory = m_parameters.Any(p => p.HasFromFactoryAttribute);
            bool hasFactoryParam = m_parameters.Any(p => p.HasFactoryParamAttribute);

            if (hasFromFactory && hasFactoryParam)
            {
                AddDiagnostic(Diagnostics.ConflictingParameterAttributesDiagnostic.Get(this));
            }

            // Redundant, but easier to read
            if (!hasFromFactory && !hasFactoryParam) {
                UsesFactoryParamMode = false;
                return;
            }
            
            UsesFactoryParamMode = hasFactoryParam;
        }

        private void VisitParameters(ParameterListSyntax parametersList)
        {
            foreach (ParameterSyntax parameter in parametersList.Parameters)
            {
                ParameterSyntaxVisitor visitor = new ParameterSyntaxVisitor(this, m_semanticModel);
                visitor.Accept(parameter);
                m_parameters.Add(visitor);

                AddChild(visitor);
            }
        }
    }
}
