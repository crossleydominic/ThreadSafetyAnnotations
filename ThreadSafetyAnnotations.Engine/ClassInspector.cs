using System;
using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;
using ThreadSafetyAnnotations.Attributes;
using ThreadSafetyAnnotations.Engine.Extensions;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine
{
    public class ClassInspector
    {
        private static readonly Type GuardedByAttributeType = typeof (GuardedByAttribute);
        private static readonly Type LockAttributeType = typeof (LockAttribute);

        private SemanticModel _semanticModel;
        private ClassWalker _walker;

        private class ClassWalker: SyntaxWalker        
        {
            private SemanticModel _semanticModel;
            private List<GuardedFieldInfo> _guardedFields;
            private List<LockInfo> _locks;

            public ClassWalker(SemanticModel semanticModel)
            {
                _semanticModel = semanticModel;
                _guardedFields = new List<GuardedFieldInfo>();
                _locks = new List<LockInfo>();
            }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                VariableDeclaratorSyntax variableDeclaration = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

                FieldSymbol field = (FieldSymbol) _semanticModel.GetDeclaredSymbol(variableDeclaration);

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

                base.VisitFieldDeclaration(node);
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                IEnumerable<AccessorDeclarationSyntax> accessors = node.DescendantNodes().OfType<AccessorDeclarationSyntax>();

                List<BlockSyntax> blocks = accessors.Select(a => a.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault()).ToList();

                base.VisitPropertyDeclaration(node);
            }

            public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            {
                IEnumerable<AccessorDeclarationSyntax> accessors = node.DescendantNodes().OfType<AccessorDeclarationSyntax>();

                List<BlockSyntax> blocks = accessors.Select(a => a.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault()).ToList();


                base.VisitIndexerDeclaration(node);
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                BlockSyntax implementationBlock = node.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault();

                base.VisitMethodDeclaration(node);
            }

            public List<LockInfo> Locks { get { return _locks; } }
            public List<GuardedFieldInfo> GuardedFields { get { return _guardedFields; } }
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

            foreach (LockInfo lockInfo in classInfo.Locks)
            {
                lockInfo.Parent = classInfo;
            }

            foreach (GuardedFieldInfo fieldInfo in classInfo.GuardedFields)
            {
                fieldInfo.Parent = classInfo;
            }

            return classInfo;
        }
    }
}