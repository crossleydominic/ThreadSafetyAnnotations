using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    public interface IBaseInfo
    {
        CommonSyntaxNode Declaration { get; }
        ISymbol Symbol { get; }
    }

    public class BaseInfo<TDecl, TSymbol> : IBaseInfo
        where TDecl : CommonSyntaxNode
        where TSymbol : ISymbol
    {
        private TDecl _declaration;
        private TSymbol _symbol;
        private ISemanticModel _semanticModel;

        public BaseInfo(
            TDecl declaration,
            ISemanticModel semanticModel)
        {
            #region Input validation

            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }

            #endregion

            _declaration = declaration;
            _semanticModel = semanticModel;
            _symbol = (TSymbol)_semanticModel.GetDeclaredSymbol(declaration);
        }

        public TDecl Declaration { get { return _declaration; } }
        public TSymbol Symbol { get { return _symbol; } }
        public ISemanticModel SemanticModel { get { return _semanticModel; } }
        CommonSyntaxNode IBaseInfo.Declaration { get { return _declaration; } }
        ISymbol IBaseInfo.Symbol { get { return _symbol; } }
    }
}
