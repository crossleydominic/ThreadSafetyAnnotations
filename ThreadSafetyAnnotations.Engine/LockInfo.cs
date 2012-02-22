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
                    Declaration,
                    Symbol));
            }

            if (!_isSystemObjectType)
            {
                issues.Add(new Issue(
                    string.Format("Declared lock '{0}' must be of type System.Object", _lockName),
                    ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT,
                    Declaration,
                    Symbol));
            }

            return issues;
        }

        private void DiscoverLockInformation()
        {
            _isPrivate = Symbol.DeclaredAccessibility == Accessibility.Private;
            _lockName = Declaration.Identifier.GetText();
            _isSystemObjectType = Symbol.Type.SpecialType == Roslyn.Compilers.SpecialType.System_Object;
        }

        public string LockName { get { return _lockName; } }
    }
}
