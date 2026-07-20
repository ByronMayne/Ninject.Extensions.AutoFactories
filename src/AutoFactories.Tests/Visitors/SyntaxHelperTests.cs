using AutoFactories.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoFactories.Tests.Visitors
{

    public class SyntaxHelperTests
    {
        [Fact]
        public Task GetValue_With_LiteralExpression_Returns_Expected()
            => ComposeAsync(
                argumentSyntax: "Name = \"Value\"",
                assert: (a, m) =>
                {
                    object? value = SyntaxHelpers.GetValue(a, m);
                    Assert.Equal("Value", value);
                });

        [Fact]
        public Task GetValue_With_InterpolatedString_Returns_Expected()
            => ComposeAsync(
                argumentSyntax: "Name = $\"{nameof(System.StringComparer)}\"",
                assert: (a, m) =>
                {
                    object? value = SyntaxHelpers.GetValue(a, m);
                    Assert.Equal("StringComparer", value);
                });

        [Fact]
        public Task GetValue_With_NameOf_Returns_Expected()
            => ComposeAsync(
                argumentSyntax: "Name = nameof(System.StringComparer)",
                assert: (a, m) =>
                {
                    object? value = SyntaxHelpers.GetValue(a, m);
                    Assert.Equal("StringComparer", value);
                });



        private async Task ComposeAsync(
            string? argumentSyntax = null,
            string[]? additionalSources = null,
            Action<AttributeArgumentSyntax, SemanticModel>? assert = null)
        {
            additionalSources ??= Array.Empty<string>();

            string source = $$"""
                [CustomAttribute({{argumentSyntax}})]
                public class TargetClass
                {}
                """;


            SyntaxTree[] syntaxTrees = [
                CSharpSyntaxTree.ParseText(source),
                ..additionalSources.Select(s => CSharpSyntaxTree.ParseText(s))
            ];

            UnitTestCompiler compiler = new UnitTestCompiler();
            CompileResult result = await compiler.CompileAsync(syntaxTrees);

            CompilationUnitSyntax compilationUnit = (CompilationUnitSyntax)syntaxTrees[0].GetRoot();
            ClassDeclarationSyntax declarationSyntax = compilationUnit.ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();

            AttributeListSyntax syntaxList = declarationSyntax.AttributeLists[0];
            AttributeSyntax attributeSyntax = syntaxList.Attributes[0];
            AttributeArgumentSyntax argument = attributeSyntax.ArgumentList.Arguments[0];
            assert(argument, result.SemanticModel);
        }
    }
}
