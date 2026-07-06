using AutoFactories.Diagnostics;
using AutoFactories.Tests.Generators;
using FluentAssertions;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class DiagnosticsTests : AbstractTest
    {
        public DiagnosticsTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddAnalyzer<AutoFactoriesAnalyzer>();
            AddGenerator<AutoFactoriesGeneratorHoist>();
        }


        [Fact]
        public Task ExposeAsNotDerived()
            => Compose($$"""
                using AutoFactories;

                [AutoFactory(ExposeAs=typeof(int))]
                public class Human
                {
                }

                """,
                assertAnalyzerResult: d => d.Should()
                    .OnlyContain(d => d.Id == DiagnosticIdentifier.ExposedAsIsNotDerivedType.Value));


        [Fact]
        public Task ExpsedAsProducesNoError()
            => Compose($$"""
                using AutoFactories;

                public interface IHuman 
                {}

                [AutoFactory(ExposeAs=typeof(IHuman))]
                public class Human : IHuman
                {
                }

                """,
                assertAnalyzerResult: d => d.Should()
                    .BeEmpty());

        [Fact]
        public Task UntaggedClass()
            => Compose($$"""
                using AutoFactories; 

                public class UntaggedFactory
                {
                    public UntaggedFactory([FromFactory] string name)
                    {}
                }
                """,
                assertAnalyzerResult: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnmarkedFactory.Value));

        public Task PublicFactory_WithInternalClass_EmitsInconsistentFactoryAccessibility()
            => Compose($$"""
                using AutoFactories;

                public partial class ChairFactory 
                {}

                [AutoFactory(typeof(ChairFactory))]
                internal class Chair 
                {}
                """,
                assertAnalyzerResult: d => d.Should()
                .OnlyContain(d => d.Id == DiagnosticIdentifier.InconsistentFactoryAccessibility.Value));

        [Fact]
        public Task Parameter_With_Unresolved_Type_EmitsUnresolvedParameterType()
        {
            AddGenerator(new AddSourceIncrementalGenerator("""
                namespace Items
                {
                    public interface IExternalType
                    {}
                }
                """, "Input.g.cs"));

            return Compose(
                """
                using Items;
                using AutoFactories;

                [AutoFactory]
                public class Provider 
                {
                    public Provider(IExternalType externalType)
                    {
                    }
                }
                """,
                assertAnalyzerResult:
                    d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnresolvedParameterType.Value));
        }
    }
}
