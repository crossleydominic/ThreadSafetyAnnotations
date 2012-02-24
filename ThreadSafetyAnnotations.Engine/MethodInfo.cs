using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine
{
    internal class MethodInfo : BaseInfo<MethodDeclarationSyntax, MethodSymbol>
    {
        private ClassInfo _parentClass;

        public MethodInfo(
            ClassInfo parentClass, 
            MethodDeclarationSyntax methodDeclaration, 
            ISemanticModel semanticModel)
            : base(methodDeclaration, semanticModel)
        {
            #region Input validation

            if (parentClass == null)
            {
                throw new ArgumentNullException("parentClass");
            }

            #endregion

            _parentClass = parentClass;
        }
    }
}
