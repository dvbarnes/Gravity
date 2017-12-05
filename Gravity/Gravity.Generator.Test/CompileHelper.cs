using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Generator.Test
{
    public static class CompileHelper
    {
        public static EmitResult Compile(this SyntaxNode node)
        {
            var workspace = new AdhocWorkspace();
            var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

            var usingDirectives = generator.NamespaceImportDeclaration("System");
            var namespaceDeclaration = generator.NamespaceDeclaration("MyTypes", node);

            var newNode = generator.CompilationUnit(usingDirectives, namespaceDeclaration)
    .NormalizeWhitespace();
            var syntaxTree = CSharpSyntaxTree.ParseText(newNode.ToString());

            CSharpCompilation compilation = CSharpCompilation.Create(
                "assemblyName",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(dllStream, pdbStream);
                return emitResult;
            }
        }

    }
}
