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
        private List<MethodInfo> _methods;
        private List<PropertyInfo> _properties;
        private bool _isMarkedWithThreadSafeAttribute;
        private bool _isPartial;
        private bool _isStatic;
        private bool _isAbstract;

        public ClassInfo(ClassDeclarationSyntax classDeclaration, ISemanticModel semanticModel)
            : base(classDeclaration, semanticModel)
        {
            _locks = new List<LockInfo>();
            _guardedMembers = new List<GuardedMemberInfo>();
            _methods = new List<MethodInfo>();
            _properties = new List<PropertyInfo>();
            _isMarkedWithThreadSafeAttribute = false;

            DiscoverClassInformation();
            DiscoverMembers<LockAttribute>((cls, var, model) => _locks.Add(new LockInfo(cls, var, model)));
            DiscoverMembers<GuardedByAttribute>((cls, var, model) => _guardedMembers.Add(new GuardedMemberInfo(cls, var, model)));
            DiscoverMethods();
            DiscoverProperties();
        }

        private void DiscoverMethods()
        {
            foreach (MethodDeclarationSyntax methodDeclaration in Declaration.DescendentNodes().OfType<MethodDeclarationSyntax>())
            {
                ISymbol methodSymbol = SemanticModel.GetDeclaredSymbol(methodDeclaration);

                _methods.Add(new MethodInfo(this, methodDeclaration, SemanticModel));
            }
        }

        private void DiscoverProperties()
        {
            foreach (PropertyDeclarationSyntax propertyDeclaration in Declaration.DescendentNodes().OfType<PropertyDeclarationSyntax>())
            {
                ISymbol propertySymbol = SemanticModel.GetDeclaredSymbol(propertyDeclaration);

                _properties.Add(new PropertyInfo(this, propertyDeclaration, SemanticModel));
            }
        }

        private void DiscoverMembers<TAttribute>(Action<ClassInfo, VariableDeclaratorSyntax, ISemanticModel> discoveredAction)
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

        private void DiscoverClassInformation()
        {
            _isMarkedWithThreadSafeAttribute = Symbol.HasCustomAttribute<ThreadSafeAttribute>();
            _isPartial = Declaration.Modifiers.Any(SyntaxKind.PartialKeyword);
            _isStatic = Declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
            _isAbstract = Declaration.Modifiers.Any(SyntaxKind.AbstractKeyword);
        }

        public bool IsMarkedWithThreadSafeAttribute { get { return _isMarkedWithThreadSafeAttribute; } }
        public bool IsPartial { get { return _isPartial; } }
        public bool IsStatic { get { return _isStatic; } }
        public bool IsAbstract { get { return _isAbstract; } }
        public List<LockInfo> Locks { get { return _locks; } }
        public List<GuardedMemberInfo> GuardedMembers { get { return _guardedMembers; } }
    }
}
