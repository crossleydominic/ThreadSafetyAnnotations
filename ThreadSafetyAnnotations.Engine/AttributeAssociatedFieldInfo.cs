using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine
{
    public class AttributeAssociatedFieldInfo : BaseInfo<FieldDeclarationSyntax, FieldSymbol>
    {
        private AttributeData _attribute;
        private ClassInfo _parent;

        public AttributeAssociatedFieldInfo(
            FieldDeclarationSyntax declaration, 
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel) : base(declaration, symbol, semanticModel)
        {
            _attribute = associatedAttribute;
        }

        public ClassInfo Parent { get { return _parent; } set { _parent = value; } }

        public AttributeData Attribute { get { return _attribute; } }
    }
}