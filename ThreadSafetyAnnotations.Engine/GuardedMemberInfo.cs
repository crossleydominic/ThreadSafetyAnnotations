using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using System.Collections.ObjectModel;

namespace ThreadSafetyAnnotations.Engine
{
    internal class GuardedMemberInfo
    {
        private VariableDeclaratorSyntax _guardedMemberDeclaration;
        private ISemanticModel _semanticModel;
        private FieldSymbol _guardedMemberSymbol;
        private ClassInfo _parentClass;

        private bool _isPrivate;
        private string _guardedMemberName;
        private List<string> _protectingLockNames;

        public GuardedMemberInfo(
            ClassInfo parentClass,
            VariableDeclaratorSyntax guardedMemberDeclaration, 
            ISemanticModel semanticModel)
        {
            #region Input validation

            if (parentClass == null)
            {
                throw new ArgumentNullException("parentClass");
            }

            if (guardedMemberDeclaration == null)
            {
                throw new ArgumentNullException("guardedMemberDeclaration");
            }

            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }

            #endregion

            _parentClass = parentClass;
            _guardedMemberDeclaration = guardedMemberDeclaration;
            _semanticModel = semanticModel;
            _guardedMemberSymbol = (FieldSymbol)_semanticModel.GetDeclaredSymbol(guardedMemberDeclaration);

            _isPrivate = false;
            _protectingLockNames = new List<string>();
        }

        public List<Issue> Analyze()
        {
            DiscoverInformation();

            List<Issue> issues = new List<Issue>();

            if (!_isPrivate)
            {
                issues.Add(new Issue(
                    string.Format("Declared guarded member '{0}' must be declared private", _guardedMemberName), 
                    ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE,
                    _guardedMemberDeclaration, 
                    _guardedMemberSymbol));
            }

            if (_protectingLockNames.Count == 0)
            {
                issues.Add(new Issue(
                    string.Format("Declared guarded member '{0}' must be protected by at least one lock", _guardedMemberName), 
                    ErrorCode.GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK,
                    _guardedMemberDeclaration, 
                    _guardedMemberSymbol));
            }

            return issues;
        }

        private void DiscoverInformation()
        {
            _isPrivate = _guardedMemberSymbol.DeclaredAccessibility == Accessibility.Private;
            _guardedMemberName = _guardedMemberDeclaration.Identifier.GetText();

            _protectingLockNames = _guardedMemberSymbol.GetGuardedByLockNames();
        }

        public string GuardedMemberName { get { return _guardedMemberName; } }
        public ReadOnlyCollection<string> ProtectingLockNames { get { return _protectingLockNames.AsReadOnly(); } }
        public FieldSymbol Symbol { get { return _guardedMemberSymbol; } }
        public VariableDeclaratorSyntax Declaration { get { return _guardedMemberDeclaration; } }
    }
}
