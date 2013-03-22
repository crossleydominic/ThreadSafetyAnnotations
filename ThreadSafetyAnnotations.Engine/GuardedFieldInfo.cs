using System.Collections.Generic;
using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine
{
    public class GuardedFieldInfo : AttributeAssociatedFieldInfo
    {
        private ClassInfo _parent;

        public GuardedFieldInfo(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel)
            : base(declaration, symbol, associatedAttribute, semanticModel) {}

    }
}