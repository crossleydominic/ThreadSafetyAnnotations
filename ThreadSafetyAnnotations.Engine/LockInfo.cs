using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    internal class LockInfo : BaseInfo<VariableDeclaratorSyntax, FieldSymbol>
    {
        private ClassInfo _parentClass;

        private bool _isPrivate;
        private string _lockName;
        private bool _isSystemObjectType;

        public LockInfo(
            ClassInfo parentClass,
            VariableDeclaratorSyntax lockDeclaration,
            ISemanticModel semanticModel)
            : base(lockDeclaration, semanticModel)
        {
            #region Input validation

            if (parentClass == null)
            {
                throw new ArgumentNullException("parentClass");
            }

            #endregion

            _parentClass = parentClass;

            _isPrivate = false;
            _lockName = string.Empty;
            _isSystemObjectType = false;

            DiscoverLockInformation();
        }

        private void DiscoverLockInformation()
        {
            _isPrivate = Symbol.DeclaredAccessibility == Accessibility.Private;
            _lockName = Declaration.Identifier.GetText();
            _isSystemObjectType = Symbol.Type.SpecialType == Roslyn.Compilers.SpecialType.System_Object;
        }

        public string LockName { get { return _lockName; } }
        public bool IsPrivate { get { return _isPrivate; } }
        public bool IsSystemObject { get { return _isSystemObjectType; } }
    }
}
