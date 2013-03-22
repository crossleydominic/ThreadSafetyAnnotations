using System;
using System.Collections.Generic;
using Roslyn.Compilers.CSharp;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.Engine
{
    public class ClassInfo : BaseInfo<ClassDeclarationSyntax, NamedTypeSymbol>
    {
        private List<GuardedFieldInfo> _guardedFields;
        private List<LockInfo> _locks;

        public ClassInfo(
            ClassDeclarationSyntax declaration, 
            SemanticModel semanticModel,
            NamedTypeSymbol symbol)
            : base(declaration, symbol, semanticModel)
        {
            _guardedFields = new List<GuardedFieldInfo>();
            _locks = new List<LockInfo>();

        }

        public List<GuardedFieldInfo> GuardedFields { get { return _guardedFields; } }
        public List<LockInfo> Locks { get { return _locks; } }
        public bool HasThreadSafeAttribute { get { return Symbol.HasCustomAttribute<ThreadSafeAttribute>(); } }
    }
}