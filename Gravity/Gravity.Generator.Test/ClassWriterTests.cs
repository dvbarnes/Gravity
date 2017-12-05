using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.IO;
using ApplicationParser;
using System.Collections.Generic;

namespace Gravity.Generator.Test
{
    [TestClass]
    public class ClassWriterTests
    {
        [TestMethod]
        [TestCategory("Generator")]
        public void CreateClass_PassInProperty_CompilesWithoutError()
        {
            //ARRANGE
            var workspace = new AdhocWorkspace();
            var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

            var clazz = new ClassWriter(generator);
            //ACT
            var result = clazz.CreateClass(new ApplicationParser.ObjectDef
            {
                Name = "hello world",
                Fields = new List<Field>
                {
                    new Field("hello", FieldTypes.FixedLength, false)
                }
            });

            //ASSERT
            var r = result.Compile();
            Assert.IsTrue(r.Success);

        }
    }
}
