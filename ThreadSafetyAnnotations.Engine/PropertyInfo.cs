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
        private AccessorDeclarationSyntax _getAccessor;
        private AccessorDeclarationSyntax _setAccessor;

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

            DiscoverInformation();
        }

        private void DiscoverInformation()
        {
            IEnumerable<AccessorDeclarationSyntax> accessors = Declaration.DescendentNodes().OfType<AccessorDeclarationSyntax>();

            _getAccessor = accessors.Where(a => a.Kind == SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
            _setAccessor = accessors.Where(a => a.Kind == SyntaxKind.SetAccessorDeclaration).FirstOrDefault();
        }

        public AccessorDeclarationSyntax GetAccessor { get { return _getAccessor; } }
        public AccessorDeclarationSyntax SetAccessor { get { return _setAccessor; } }

        public bool HasGet { get { return _getAccessor != null; } }
        public bool HasSet { get { return _setAccessor != null; } }
    }
}
