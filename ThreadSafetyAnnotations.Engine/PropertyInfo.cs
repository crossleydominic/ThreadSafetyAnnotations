using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    internal class PropertyInfo : BaseInfo<PropertyDeclarationSyntax, PropertySymbol>
    {
        private ClassInfo _parentClass;

        public PropertyInfo(
            ClassInfo parentClass, 
            PropertyDeclarationSyntax PropertyDeclaration, 
            ISemanticModel semanticModel)
            : base(PropertyDeclaration, semanticModel)
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
