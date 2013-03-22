using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine
{
    public class BaseInfo<TDecl, TSym> 
        where TDecl : MemberDeclarationSyntax 
        where TSym : Symbol
    {
        private TDecl _declaration;
        private TSym _symbol;
        private SemanticModel _semanticModel;

        public BaseInfo(TDecl declaration, TSym symbol, SemanticModel semanticModel)
        {
            _declaration = declaration;
            _semanticModel = semanticModel;
            _symbol = symbol;
        }

        public TDecl Declaration { get { return _declaration; } }
        public TSym Symbol { get { return _symbol; } }

        public SemanticModel SemanticModel { get { return _semanticModel; } }
    }
}