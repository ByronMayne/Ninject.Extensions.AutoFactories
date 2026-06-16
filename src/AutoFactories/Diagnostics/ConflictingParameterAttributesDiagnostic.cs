using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal class ConflictingParameterAttributesDiagnostic : SyntaxDiagnosticBuilder<ConstructorDeclarationVisitor>
    {
        public ConflictingParameterAttributesDiagnostic()
            : base(
                  id: DiagnosticIdentifier.ConflictingParameterAttributes,
                  title: "Conflicting Parameter Attributes",
                  category: "Usage",
                  messageFormat:
                    "The constructor for '{0}' uses both [FromFactory] and [FactoryParam] attributes. " +
                    "These attributes are mutually exclusive. Use [FromFactory] to mark parameters that should be " +
                    "resolved from the DI container, OR use [FactoryParam] to mark parameters that should be " +
                    "passed to the factory method. Do not mix both in the same constructor.")
        {
            Severity = DiagnosticSeverity.Error;
        }

        public override Diagnostic Create(ConstructorDeclarationVisitor visitor)
        {
            return Diagnostic.Create(Descriptor, visitor.Location, new object?[] {
                   visitor.Class.Type.Name
            });
        }

        public static Diagnostic Get(ConstructorDeclarationVisitor visitor)
        {
            ConflictingParameterAttributesDiagnostic builder = new ConflictingParameterAttributesDiagnostic();
            return builder.Create(visitor);
        }
    }
}
