using System;
using System.Linq;
using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine.Info
{
    public class GuardedFieldInfo : AttributeAssociatedFieldInfo
    {
        private LockHierarchy _declaredLockHierarchy;

        public GuardedFieldInfo(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel)
            : base(declaration, symbol, associatedAttribute, semanticModel)
        {
            _declaredLockHierarchy = LockHierarchy.FromStringList(Attribute.ConstructorArguments.SelectMany(arg => arg.Values.Select(argVal => argVal.Value.ToString())).ToList());
        }

        public string Name { get { return Symbol.Name; } }

        public LockHierarchy DeclaredLockHierarchy { get { return _declaredLockHierarchy; } }
    }
}