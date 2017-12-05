using ApplicationParser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Generator
{
    public class Class1
    {
        public static void Main(string[] args)
        {
            //var parser = new ApplicationParser.Parser();
            //var application = parser.Parse("");
            var workspace = new AdhocWorkspace();
            var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);
            var p = new ClassWriter(generator);
            var result = p.CreateClass(new ApplicationParser.ObjectDef
            {
                Name = "hello world",
                Fields = new List<Field>
            {
                new Field("hello", FieldTypes.FixedLength, false)
            }
            });
            // Create using/Imports directives
            var usingDirectives = generator.NamespaceImportDeclaration("System");
            // Declare a namespace
            var namespaceDeclaration = generator.NamespaceDeclaration("MyTypes", result);

            // Get a CompilationUnit (code file) for the generated code
            var newNode2 = generator.CompilationUnit(usingDirectives, namespaceDeclaration)
                .NormalizeWhitespace();

            var syntaxTree = CSharpSyntaxTree.ParseText(newNode2.ToString());

            CSharpCompilation compilation = CSharpCompilation.Create(
                "assemblyName",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(dllStream, pdbStream);
                if (!emitResult.Success)
                {
                    foreach (var d in emitResult.Diagnostics)
                    {
                        Console.Error.WriteLine(d.ToString());
                    }
                    // emitResult.Diagnostics
                }
            }
        }
    }
}
