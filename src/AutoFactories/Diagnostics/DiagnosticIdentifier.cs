namespace AutoFactories.Diagnostics
{

    internal struct DiagnosticIdentifier
    {
        public static DiagnosticIdentifier UnmarkedFactory = new DiagnosticIdentifier("AF1001");
        public static DiagnosticIdentifier InconsistentFactoryAccessibility = new DiagnosticIdentifier("AF1002");
        public static DiagnosticIdentifier ExposedAsIsNotDerivedType = new DiagnosticIdentifier("AF1003");
        public static DiagnosticIdentifier UnresolvedParameterType = new DiagnosticIdentifier("AF1004");

        public readonly string Value;

        private DiagnosticIdentifier(string value)
        {
            Value = value;
        }

        public static bool operator ==(DiagnosticIdentifier left, DiagnosticIdentifier right)
            => left.Value == right.Value;

        public static bool operator !=(DiagnosticIdentifier left, DiagnosticIdentifier right)
            => left.Value != right.Value;

        public static bool operator ==(DiagnosticIdentifier left, string right)
            => left.Value == right;

        public static bool operator !=(DiagnosticIdentifier left, string right) => left.Value != right;
    }
}