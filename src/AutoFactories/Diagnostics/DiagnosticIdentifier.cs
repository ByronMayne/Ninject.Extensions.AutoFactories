using Vogen;

namespace AutoFactories.Diagnostics
{
    [Instance("UnmarkedFactory", "AF1001")]
    [Instance("InconsistentFactoryAccessibility", "AF1002")]
    [Instance("ExposedAsIsNotDerivedType", "AF1003")]
    [Instance("UnresolvedParameterType", "AF1004")]
    [Instance("ConflictingParameterAttributes", "AF1005")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    internal partial record DiagnosticIdentifier
    {}
}
