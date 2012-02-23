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

        private void DiscoverInformation()
        {
            _isPrivate = Symbol.DeclaredAccessibility == Accessibility.Private;
            _guardedMemberName = Declaration.Identifier.GetText();
            _protectingLockNames = Symbol.GetGuardedByLockNames();
        }

        public string GuardedMemberName { get { return _guardedMemberName; } }
        public ReadOnlyCollection<string> ProtectingLockNames { get { return _protectingLockNames.AsReadOnly(); } }
        public bool IsPrivate { get { return _isPrivate; } }
    }
}
