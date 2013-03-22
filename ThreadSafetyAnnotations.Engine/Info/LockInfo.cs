using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine.Info
{
    public class LockInfo : AttributeAssociatedFieldInfo
    {
        public LockInfo(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel)
            : base(declaration, symbol, associatedAttribute, semanticModel)
        {
        }

        public string Name { get { return Symbol.Name; } }
    }
}