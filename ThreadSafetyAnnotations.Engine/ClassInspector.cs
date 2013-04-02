using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Roslyn.Compilers.CSharp;
using ThreadSafetyAnnotations.Attributes;
using ThreadSafetyAnnotations.Engine.Extensions;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine
{
    public class ClassInspector
    {
        private SemanticModel _semanticModel;
        private ClassWalker _walker;

        private class ClassWalker: SyntaxWalker        
        {
            private SemanticModel _semanticModel;
            private List<GuardedFieldInfo> _guardedFields;
            private List<LockInfo> _locks;
            private List<MemberInfo> _members;

            public ClassWalker(SemanticModel semanticModel)
            {
                _semanticModel = semanticModel;
                _guardedFields = new List<GuardedFieldInfo>();
                _locks = new List<LockInfo>();
                _members = new List<MemberInfo>();
            }

            private void VisitIndexerOrPropertyDeclarationSyntax(MemberDeclarationSyntax node)
            {
                IEnumerable<BlockSyntax> accessorBlocks = node.DescendantNodes()
                    .OfType<AccessorDeclarationSyntax>()
                    .Select(a => a.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault());

                Symbol symbol = _semanticModel.GetDeclaredSymbol(node);
                if (symbol != null)
                {
                    _members.Add(new MemberInfo(node, symbol, _semanticModel, accessorBlocks));
                }
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                BlockSyntax accessorBlock = node.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault();

                Symbol symbol = _semanticModel.GetDeclaredSymbol(node);

                if (symbol != null)
                {
                    _members.Add(new MemberInfo(node, symbol, _semanticModel, new List<BlockSyntax>()
                    {
                        accessorBlock
                    }));
                }

                base.VisitMethodDeclaration(node);
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                VisitIndexerOrPropertyDeclarationSyntax(node);
                
                base.VisitPropertyDeclaration(node);
            }

            public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            {
                VisitIndexerOrPropertyDeclarationSyntax(node);

                base.VisitIndexerDeclaration(node);
            }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                VariableDeclaratorSyntax variableDeclaration = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

                FieldSymbol field = (FieldSymbol) _semanticModel.GetDeclaredSymbol(variableDeclaration);

                if (field != null)
                {

                    var attributes = field.GetAttributes();

                    foreach (AttributeData attribute in attributes)
                    {
                        if (attribute.IsInstanceOfAttributeType<GuardedByAttribute>())
                        {
                            GuardedFieldInfo guardedField = new GuardedFieldInfo(node, field, attribute, _semanticModel);

                            _guardedFields.Add(guardedField);
                        }
                        else if (attribute.IsInstanceOfAttributeType<LockAttribute>())
                        {
                            LockInfo lockField = new LockInfo(node, field, attribute, _semanticModel);

                            _locks.Add(lockField);
                        }
                        else
                        {
                            //do nothing
                        }
                    }
                }
                base.VisitFieldDeclaration(node);
            }
            
            public List<LockInfo> Locks { get { return _locks; } }
            public List<GuardedFieldInfo> GuardedFields { get { return _guardedFields; } }
            public List<MemberInfo> Members { get { return _members; } }
        }

        public ClassInspector(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _walker = new ClassWalker(_semanticModel);
        }

        public ClassInfo GetClassInfo(ClassDeclarationSyntax classDeclaration)
        {
            _walker.Visit(classDeclaration);
            
            ClassInfo classInfo = new ClassInfo(classDeclaration, _semanticModel, _semanticModel.GetDeclaredSymbol(classDeclaration));

            classInfo.Locks.AddRange(_walker.Locks);
            classInfo.GuardedFields.AddRange(_walker.GuardedFields);
            classInfo.Members.AddRange(_walker.Members);

            foreach (LockInfo lockInfo in classInfo.Locks)
            {
                lockInfo.Parent = classInfo;
            }

            foreach (GuardedFieldInfo fieldInfo in classInfo.GuardedFields)
            {
                fieldInfo.Parent = classInfo;
            }

            foreach (MemberInfo memberInfo in classInfo.Members)
            {
                memberInfo.Parent = classInfo;
            }

            return classInfo;
        }
    }
}