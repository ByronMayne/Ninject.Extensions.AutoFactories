using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoFactories.Tests
{
    public class CodeAnalysisVersionTests
    {
        /// <summary>
        /// Microsoft.CodeAnalysis.CSharp is intentionally pinned to 4.8.0 in Directory.Packages.props so
        /// that AutoFactories stays loadable in older Visual Studio / Roslyn hosts. This test fails on
        /// purpose if the referenced version changes, forcing the change to be made explicit here.
        /// </summary>
        [Fact]
        public void MicrosoftCodeAnalysisCSharp_IsPinnedTo_4_8_0()
        {
            Version? actual = typeof(CSharpSyntaxTree).Assembly.GetName().Version;

            actual.Should().Be(new Version(4, 8, 0, 0), because:
                "Microsoft.CodeAnalysis.CSharp is pinned to 4.8.0 in Directory.Packages.props to support " +
                "older versions of Visual Studio. If you intentionally change the pin, update this test to " +
                "match the new version.");
        }
    }
}
