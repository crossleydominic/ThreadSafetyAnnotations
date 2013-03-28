using System.Collections.Generic;
using Roslyn.Compilers.CSharp;
using ThreadSafetyAnnotations.Attributes;
using ThreadSafetyAnnotations.Engine.Extensions;

namespace ThreadSafetyAnnotations.Engine.Info
{
    public class ClassInfo : BaseInfo<ClassDeclarationSyntax, NamedTypeSymbol>
    {
        private List<GuardedFieldInfo> _guardedFields;
        private List<LockInfo> _locks;
        private List<MemberInfo> _members;

        public ClassInfo(
            ClassDeclarationSyntax declaration, 
            SemanticModel semanticModel,
            NamedTypeSymbol symbol)
            : base(declaration, symbol, semanticModel)
        {
            _guardedFields = new List<GuardedFieldInfo>();
            _locks = new List<LockInfo>();
            _members = new List<MemberInfo>();
        }

        public List<GuardedFieldInfo> GuardedFields { get { return _guardedFields; } }
        public List<LockInfo> Locks { get { return _locks; } }
        public List<MemberInfo> Members { get { return _members; } }
        public bool HasThreadSafeAttribute { get { return Symbol.HasCustomAttribute<ThreadSafeAttribute>(); } }
    }
}