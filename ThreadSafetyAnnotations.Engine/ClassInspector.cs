using System;
using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;
using ThreadSafetyAnnotations.Attributes;

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

                FieldSymbol field = (FieldSymbol)_semanticModel.GetDeclaredSymbol(variableDeclaration);

                var attributes = field.GetAttributes();

                foreach (AttributeData attribute in attributes)
                {
                    if (string.Equals(attribute.AttributeClass.ContainingAssembly.Name, GuardedByAttributeType.Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(attribute.AttributeClass.Name, GuardedByAttributeType.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            GuardedFieldInfo guardedField = new GuardedFieldInfo(node, field, attribute, _semanticModel);

                            _guardedFields.Add(guardedField);
                        }
                        else if (string.Equals(attribute.AttributeClass.Name, LockAttributeType.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            LockInfo lockField = new LockInfo(node,field, attribute, _semanticModel);

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