using AutoFactories.Diagnostics;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;
using System.Collections.Immutable;
using System.Linq;

namespace AutoFactories
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoFactoriesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public AutoFactoriesAnalyzer()
        {
            SupportedDiagnostics = [
                new UnmarkedFactoryDiagnostic().Descriptor,
                new InconsistentFactoryAccessibilityBuilder().Descriptor,
                new ExposedAsNotDerivedTypeDiagnostic().Descriptor,
                new UnresolvedParameterTypeDiagnostic().Descriptor,
                new ConflictingParameterAttributesDiagnostic().Descriptor,
             ];
        }


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction((context) =>
            {
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
                ClassDeclarationVisitor visitor = new(true, context.SemanticModel);
                visitor.Accept(classDeclarationSyntax);
                Analyze(visitor, context);
            }, SyntaxKind.ClassDeclaration);
        }

        private void Analyze(ClassDeclarationVisitor visitor, SyntaxNodeAnalysisContext context)
        {
            foreach(Diagnostic diagnostic in visitor.GetDiagnostics())
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
