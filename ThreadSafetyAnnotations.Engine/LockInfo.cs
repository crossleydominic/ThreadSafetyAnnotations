using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine
{
    internal class LockInfo
    {
        private VariableDeclaratorSyntax _lockDeclaration;
        private ISemanticModel _semanticModel;
        private FieldSymbol _lockSymbol;
        private ClassInfo _parentClass;

        private bool _isPrivate;
        private string _lockName;
        private bool _isSystemObjectType;

        public LockInfo(
            ClassInfo parentClass,
            VariableDeclaratorSyntax lockDeclaration, 
            ISemanticModel semanticModel)
        {
            #region Input validation

            if (parentClass == null)
            {
                throw new ArgumentNullException("parentClass");
            }

            if (lockDeclaration == null)
            {
                throw new ArgumentNullException("lockDeclaration");
            }

            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }

            #endregion

            _parentClass = parentClass;
            _lockDeclaration = lockDeclaration;
            _semanticModel = semanticModel;
            _lockSymbol = (FieldSymbol)_semanticModel.GetDeclaredSymbol(lockDeclaration);

            _isPrivate = false;
            _lockName = string.Empty;
            _isSystemObjectType = false;
        }

        public List<Issue> Analyze()
        {
            DiscoverLockInformation();

            List<Issue> issues = new List<Issue>();

            if (!_isPrivate)
            {
                issues.Add(new Issue(
                    string.Format("Declared lock '{0}' must be declared private", _lockName), 
                    ErrorCode.LOCK_IS_NOT_PRIVATE,
                    _lockDeclaration, 
                    _lockSymbol));
            }

            if (!_isSystemObjectType)
            {
                issues.Add(new Issue(
                    string.Format("Declared lock '{0}' must be of type System.Object", _lockName), 
                    ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT,
                    _lockDeclaration,
                    _lockSymbol));
            }

            return issues;
        }

        private void DiscoverLockInformation()
        {
            _isPrivate = _lockSymbol.DeclaredAccessibility == Accessibility.Private;
            _lockName = _lockDeclaration.Identifier.GetText();
            _isSystemObjectType = _lockSymbol.Type.SpecialType == Roslyn.Compilers.SpecialType.System_Object;
        }

        public string LockName { get { return _lockName; } }
        public FieldSymbol Symbol { get { return _lockSymbol; } }
        public VariableDeclaratorSyntax Declaration { get { return _lockDeclaration; } }
    }
}
