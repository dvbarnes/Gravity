using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace Gravity.Generator
{
    public class ClassWriter
    {
        private readonly SyntaxGenerator _generator;

        public ClassWriter(SyntaxGenerator generator)
        {
            _generator = generator;
        }
        public SyntaxNode CreateClass(ApplicationParser.ObjectDef obj)
        {
            var props = obj.Fields.Select(x => GetProperty(x)).SelectMany(x => x).ToList();


            // Generate the class' constructor
            var constructor = _generator.ConstructorDeclaration(obj.Name,
              null, Accessibility.Public,
              statements: null);

            //props.Add(constructor);
            var classDefinition = _generator.ClassDeclaration(
                                obj.Name, typeParameters: null,
                                accessibility: Accessibility.Public,
                                modifiers: DeclarationModifiers.Partial,
                                baseType: null, //need to be type BaseDTO from gravity
                                interfaceTypes: new SyntaxNode[] { },
                                members: props);

            return classDefinition;
        }
        protected virtual SyntaxNode[] GetProperty(ApplicationParser.Field field)
        {
            var propType = SpecialType.System_String;
            // Generate two private fields
            var backingField = _generator.FieldDeclaration($"_{field.Name}",
              _generator.TypeExpression(propType),
              Accessibility.Private);

            // Generate two properties with explicit get/set
            var backingFieldProperty = _generator.PropertyDeclaration(field.Name,
              _generator.TypeExpression(propType), Accessibility.Public,
              getAccessorStatements: new SyntaxNode[]
              { _generator.ReturnStatement(_generator.IdentifierName($"_{field.Name}")) },
              setAccessorStatements: new SyntaxNode[]
              { _generator.AssignmentStatement(_generator.IdentifierName($"_{field.Name}"),
                _generator.IdentifierName("value"))});

            return new[] { backingField, backingFieldProperty };
        }
    }
}
