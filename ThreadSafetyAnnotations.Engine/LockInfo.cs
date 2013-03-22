using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine
{
    public class LockInfo : AttributeAssociatedFieldInfo
    {
        private ClassInfo _parent;

        public LockInfo(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel) 
            : base(declaration, symbol, associatedAttribute, semanticModel) {}

    }
}