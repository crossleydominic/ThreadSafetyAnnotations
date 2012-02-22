using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Attributes;

namespace ThreadSafetyAnnotations.Engine
{
    internal class ClassInfo : BaseInfo<ClassDeclarationSyntax, ISymbol>
    {
        private List<LockInfo> _locks;
        private List<GuardedMemberInfo> _guardedMembers;
        private bool _isMarkedWithThreadSafeAttribute;
        private bool _isPartialClass;

        public ClassInfo(ClassDeclarationSyntax classDeclaration, ISemanticModel semanticModel)
            : base(classDeclaration, semanticModel)
        {
            _locks = new List<LockInfo>();
            _guardedMembers = new List<GuardedMemberInfo>();
            _isMarkedWithThreadSafeAttribute = false;
        }

        public List<Issue> Analyze()
        {
            List<Issue> issues = new List<Issue>();

            DiscoverClassInformation();
            DiscoverMemberInformation<LockAttribute>((cls, var, model) => _locks.Add(new LockInfo(cls, var, model)));
            DiscoverMemberInformation<GuardedByAttribute>((cls, var, model) => _guardedMembers.Add(new GuardedMemberInfo(cls, var, model)));

            issues.AddRange(AnalyzeLocks());
            issues.AddRange(AnalyzeGuardedMembers());
            issues.AddRange(AnalyzeClass());

            return issues;
        }

        private List<Issue> AnalyzeClass()
        {
            List<Issue> issues = new List<Issue>();

            issues.AddRange(AnalyzeLockUsage());

            if (_guardedMembers.Count > 0 &&
                _isMarkedWithThreadSafeAttribute == false)
            {
                issues.Add(new Issue(
                    "Class is not marked with the ThreadSafeAttribute but contains a guarded member.",
                    ErrorCode.GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS,
                    Declaration,
                    Symbol));
            }

            if (_locks.Count > 0 &&
                _isMarkedWithThreadSafeAttribute == false)
            {
                issues.Add(new Issue(
                    "Class is not marked with the ThreadSafeAttribute but contains a lock.",
                    ErrorCode.LOCK_IN_A_NON_THREAD_SAFE_CLASS,
                    Declaration,
                    Symbol));
            }


            return issues;
        }

        private List<Issue> AnalyzeLockUsage()
        {
            List<Issue> issues = new List<Issue>();

            //Check that all guards reference a named lock.
            foreach (GuardedMemberInfo guardedMember in _guardedMembers)
            {
                foreach (string protectingLockname in guardedMember.ProtectingLockNames)
                {
                    if (!_locks.Any(l => l.LockName == protectingLockname))
                    {
                        issues.Add(new Issue(
                            string.Format("Declared member '{0}' references unknown lock '{1}'", guardedMember.GuardedMemberName, protectingLockname),
                            ErrorCode.GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK,
                            guardedMember.Declaration,
                            guardedMember.Symbol));
                    }
                }
            }

            //Check that all locks actually protect something
            foreach (LockInfo lockInfo in _locks)
            {
                if (!_guardedMembers.Any(g => g.ProtectingLockNames.Any(p => p == lockInfo.LockName)))
                {
                    issues.Add(new Issue(
                        string.Format("Declared lock '{0}' is not guarding any member.", lockInfo.LockName),
                        ErrorCode.LOCK_PROTECTS_NOTHING,
                        lockInfo.Declaration,
                        lockInfo.Symbol));
                }
            }

            return issues;
        }

        private List<Issue> AnalyzeGuardedMembers()
        {
            List<Issue> issues = new List<Issue>();
            foreach (GuardedMemberInfo guardedMemberInfo in _guardedMembers)
            {
                issues.AddRange(guardedMemberInfo.Analyze());
            }

            return issues;
        }

        private List<Issue> AnalyzeLocks()
        {
            List<Issue> issues = new List<Issue>();
            foreach (LockInfo lockInfo in _locks)
            {
                issues.AddRange(lockInfo.Analyze());
            }

            return issues;
        }


        private void DiscoverMemberInformation<TAttribute>(Action<ClassInfo, VariableDeclaratorSyntax, ISemanticModel> discoveredAction)
        {
            foreach (MemberDeclarationSyntax memberDeclaration in Declaration.DescendentNodes().OfType<MemberDeclarationSyntax>())
            {
                ISymbol memberSymbol = SemanticModel.GetDeclaredSymbol(memberDeclaration);

                if (memberSymbol.HasCustomAttribute<TAttribute>())
                {
                    foreach (VariableDeclarationSyntax variableDeclarationSet in memberDeclaration.ChildNodes().OfType<VariableDeclarationSyntax>())
                    {
                        foreach (VariableDeclaratorSyntax variableDeclaration in variableDeclarationSet.Variables)
                        {
                            discoveredAction(this, variableDeclaration, SemanticModel);
                        }
                    }
                }
            }
        }

        /*private void DiscoverGuardedMemberInformation()
        {
            foreach (MemberDeclarationSyntax memberDeclaration in _classDeclaration.DescendentNodes().OfType<MemberDeclarationSyntax>())
            {
                ISymbol memberSymbol = _semanticModel.GetDeclaredSymbol(memberDeclaration);

                if (memberSymbol.HasCustomAttribute<GuardedByAttribute>())
                {
                    foreach (VariableDeclarationSyntax variableDeclarationSet in memberDeclaration.ChildNodes().OfType<VariableDeclarationSyntax>())
                    {
                        foreach (VariableDeclaratorSyntax variableDeclaration in variableDeclarationSet.Variables)
                        {
                            _guardedMembers.Add(new GuardedMemberInfo(this, variableDeclaration, _semanticModel));
                        }
                    }
                }
            }
        }

        private void DiscoverLockInformation()
        {
            foreach (MemberDeclarationSyntax memberDeclaration in _classDeclaration.DescendentNodes().OfType<MemberDeclarationSyntax>())
            {
                ISymbol memberSymbol = _semanticModel.GetDeclaredSymbol(memberDeclaration);

                if (memberSymbol.HasCustomAttribute<LockAttribute>())
                {
                    foreach (VariableDeclarationSyntax variableDeclarationSet in memberDeclaration.ChildNodes().OfType<VariableDeclarationSyntax>())
                    {
                        foreach (VariableDeclaratorSyntax variableDeclaration in variableDeclarationSet.Variables)
                        {
                            _locks.Add(new LockInfo(this, variableDeclaration, _semanticModel));
                        }
                    }
                }
            }
        }*/

        private void DiscoverClassInformation()
        {
            _isMarkedWithThreadSafeAttribute = Symbol.HasCustomAttribute<ThreadSafeAttribute>();
            _isPartialClass = false; //TODO: Find this out.
        }

        public bool IsMarkedWithThreadSafeAttribute { get { return _isMarkedWithThreadSafeAttribute; } }
        public bool IsPartialClass { get { return _isPartialClass; } }
        public List<LockInfo> Locks { get { return _locks; } }
        public List<GuardedMemberInfo> GuardedMembers { get { return _guardedMembers; } }
    }
}
