using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    public class SourceCodeElementInfo<T> where T : CommonSyntaxNode
    {
        private T _declaration;
        private ISymbol _symbol;
        private ISemanticModel _semanticModel;

        public SourceCodeElementInfo(
            T declaration,
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
            _symbol = _semanticModel.GetDeclaredSymbol(declaration);
        }

        protected T Declaration { get { return _declaration; } }
        protected ISymbol Symbol { get { return _symbol; } }
        protected ISemanticModel SemanticModel { get { return _semanticModel; } }
    }
}
