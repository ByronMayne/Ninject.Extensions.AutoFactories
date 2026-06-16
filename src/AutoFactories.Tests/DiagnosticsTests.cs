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
                    .OnlyContain(d => d.Id == DiagnosticIdentifier.ExposedAsIsNotDerivedType));


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
                assertAnalyzerResult: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnmarkedFactory));

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
                .OnlyContain(d => d.Id == DiagnosticIdentifier.InconsistentFactoryAccessibility));

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
                    d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnresolvedParameterType));
        }

        [Fact]
        public Task Conflicting_Attributes_FromFactory_And_FactoryParam()
            => Compose("""
                using AutoFactories;

                [AutoFactory]
                public class Widget
                {
                    public Widget([FromFactory] string name, [FactoryParam] int count)
                    {
                    }
                }
                """,
                assertAnalyzerResult: d => d.Should()
                    .OnlyContain(d => d.Id == DiagnosticIdentifier.ConflictingParameterAttributes));

        [Fact]
        public Task FactoryParam_Only_Produces_No_Error()
            => Compose("""
                using AutoFactories;

                [AutoFactory]
                public class Widget
                {
                    public Widget([FactoryParam] string name, int count)
                    {
                    }
                }
                """,
                assertAnalyzerResult: d => d.Should().BeEmpty());
    }
}
