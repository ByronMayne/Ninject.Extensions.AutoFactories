using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal class UnmarkedFactoryDiagnostic : DiagnosticBuilder
    {
        public UnmarkedFactoryDiagnostic() : base(
            id: DiagnosticIdentifier.UnmarkedFactory,
            title: "Unmarked FactoryView",
            category: "Code",
            messageFormat:
                $"The parameter '{{0}}' is marked with the [{TypeNames.FromFactoryAttributeType.Name}] attribute, which indicates it should be used in a factory. However, the declaring class '{{1}}' is missing the [{TypeNames.ClassAttributeType.Name}] attribute, which is required to mark it as a factory. " +
                $"To resolve this, either remove the [{TypeNames.FromFactoryAttributeType.Name}] attribute from the parameter or add the [{TypeNames.ClassAttributeType.Name}] attribute to the class '{{1}}'. This ensures the factory is properly recognized by the source generator.")
        {
            Severity = DiagnosticSeverity.Warning;
        }

        public static Diagnostic Create(ParameterSyntaxVisitor parameter)
        {
            UnmarkedFactoryDiagnostic builder = new UnmarkedFactoryDiagnostic();

            return Diagnostic.Create(builder.Descriptor, 
                parameter.AttributeLocation, 
                new object?[] { parameter.Name, parameter.Constructor.Class.Type.Name });
        }
    }

}
