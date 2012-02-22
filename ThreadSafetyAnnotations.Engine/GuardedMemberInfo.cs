using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using System.Collections.ObjectModel;

namespace ThreadSafetyAnnotations.Engine
{
    internal class GuardedMemberInfo : BaseInfo<VariableDeclaratorSyntax, FieldSymbol>
    {
        private ClassInfo _parentClass;

        private bool _isPrivate;
        private string _guardedMemberName;
        private List<string> _protectingLockNames;

        public GuardedMemberInfo(
            ClassInfo parentClass,
            VariableDeclaratorSyntax guardedMemberDeclaration,
            ISemanticModel semanticModel)
            : base(guardedMemberDeclaration, semanticModel)
        {
            #region Input validation

            if (parentClass == null)
            {
                throw new ArgumentNullException("parentClass");
            }

            #endregion

            _parentClass = parentClass;

            _isPrivate = false;
            _protectingLockNames = new List<string>();

            DiscoverInformation();
        }

        /*public List<Issue> Analyze()
        {
            DiscoverInformation();

            List<Issue> issues = new List<Issue>();

            if (!_isPrivate)
            {
                issues.Add(new Issue(
                    string.Format("Declared guarded member '{0}' must be declared private", _guardedMemberName),
                    ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE,
                    Declaration,
                    Symbol));
            }

            if (_protectingLockNames.Count == 0)
            {
                issues.Add(new Issue(
                    string.Format("Declared guarded member '{0}' must be protected by at least one lock", _guardedMemberName),
                    ErrorCode.GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK,
                    Declaration,
                    Symbol));
            }

            return issues;
        }*/

        private void DiscoverInformation()
        {
            _isPrivate = Symbol.DeclaredAccessibility == Accessibility.Private;
            _guardedMemberName = Declaration.Identifier.GetText();

            _protectingLockNames = Symbol.GetGuardedByLockNames();
        }

        public string GuardedMemberName { get { return _guardedMemberName; } }
        public ReadOnlyCollection<string> ProtectingLockNames { get { return _protectingLockNames.AsReadOnly(); } }
    }
}
