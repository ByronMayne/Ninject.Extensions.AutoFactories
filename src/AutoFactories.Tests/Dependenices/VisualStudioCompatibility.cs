using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using SGF;
using System.Reflection;

namespace AutoFactories.Tests.Dependenices
{
    /// <summary>
    /// </summary>
    public class VisualStudioCompatibility
    {
        /// <summary>
        /// Microsoft.CodeAnalysis.CSharp is intentionally pinned to a specific version in Directory.Packages.props so
        /// that we can support older versions of Visual Studio. This test fails on purpose if the referenced version changes, 
        /// forcing the change to be made explicit here.
        /// </summary>
        [Fact]
        public void MicrosoftCodeAnalysisCSharp_IsPinnedTo_SpecificVersion()
        {
            Assembly assembly = typeof(CSharpSyntaxTree).Assembly;
            AssemblyName assemblyName = assembly.GetName();
            Version? assemblyVersion = assemblyName.Version;

            string visualStudioVersion = "17.8";
            Version pinnedVersion = new Version(4, 8, 0, 0);
            assemblyVersion.Should().Be(pinnedVersion, because: $"Microsoft.CodeAnalysis.CSharp is pinned to {pinnedVersion} in Directory.Packages.props to support " +
                $"older versions of Visual Studio starting at {visualStudioVersion}. If you intentionally change the pin, update this test to " +
                "match the new version.");
        }

        /// <summary>
        /// This library can't be part of the resources otherwise Visual Studio will fail to load the analyzer. 
        /// </summary>
        [Fact]
        public void MicrosoftCodeAnalysis_Assembly_AreNotEmbedded()
        {
            Assembly assembly = typeof(AutoFactoriesAnalyzer).Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();

            resourceNames.Should()
                .NotContain(f => f.Contains("Microsoft.CodeAnalysis", StringComparison.OrdinalIgnoreCase),
                    because: """
                        Microsoft.CodeAnalysis.CSharp should not be embedded in the AutoFactories assembly.
                        If you intentionally change this, update this test to match the new behavior.
                        """);
        }
    }
}
