using System.Reflection;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Attributes;
using ThreadSafetyAnnotations.Engine.Rules;

namespace ThreadSafetyAnnotations.Engine
{
    public static class Extensions
    {
        public static IEnumerable<SyntaxNode> OfType(this IEnumerable<SyntaxNode> nodes, Type ofType)
        {
            return nodes.Where(x => x.GetType() == ofType);
        }
    }

    public class AnalysisEngineEx
    {
        private CommonSyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        private IAnalysisRuleProvider _ruleProvider;

        public AnalysisEngineEx(CommonSyntaxTree syntaxTree, SemanticModel semanticModel) : this (syntaxTree, semanticModel, new AnalysisRuleProvider()){}

        public AnalysisEngineEx(CommonSyntaxTree syntaxTree, SemanticModel semanticModel, IAnalysisRuleProvider ruleProvider)
        {
            #region Input validation

            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }

            if (semanticModel == null)
            {
                throw new ArgumentNullException("semanticModel");
            }

            #endregion

            _syntaxTree = syntaxTree;
            _semanticModel = semanticModel;
            _ruleProvider = ruleProvider;
        }

        public List<Issue> Analyze()
        {
            var diagnostics = _semanticModel.GetDiagnostics();
            var declarationDiagnostics = _semanticModel.GetDeclarationDiagnostics();

            if (diagnostics.Any(d => d.Info.Severity == DiagnosticSeverity.Error) ||
                declarationDiagnostics.Any(d=>d.Info.Severity == DiagnosticSeverity.Error))
            {
                throw new InvalidOperationException("Pre-existing errors in compilation");
            }

            List<ClassDeclarationSyntax> classDeclarations = _syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            List<ClassInfoEx> classInfos = InspectClassDeclarations(classDeclarations);

            List<Issue> issues = new List<Issue>();

            foreach (ClassInfoEx classInfo in classInfos)
            {
                IEnumerable<Issue> localIssues = _ruleProvider.Rules.SelectMany(rule => rule.AnalyzeEx(_syntaxTree, _semanticModel, classInfo));

                issues.AddRange(localIssues);
            }

            return issues;
        }

        private List<ClassInfoEx> InspectClassDeclarations(List<ClassDeclarationSyntax> classDeclarations)
        {
            List<ClassInfoEx> classInfos = new List<ClassInfoEx>();

            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                ClassInspectorEx inspector = new ClassInspectorEx(_semanticModel);

                classInfos.Add(inspector.GetClassInfo(classDeclaration));
            }

            return classInfos;
        }
    }

    public class ClassInspectorEx
    {
        private static readonly Type GuardedByAttributeType = typeof (GuardedByAttribute);
        private static readonly Type LockAttributeType = typeof (LockAttribute);

        private SemanticModel _semanticModel;
        private ClassWalker _walker;

        private class ClassWalker: SyntaxWalker        
        {
            private SemanticModel _semanticModel;
            private List<GuardedFieldInfoEx> _guardedFields;
            private List<LockInfoEx> _locks;

            public ClassWalker(SemanticModel semanticModel)
            {
                _semanticModel = semanticModel;
                _guardedFields = new List<GuardedFieldInfoEx>();
                _locks = new List<LockInfoEx>();
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
                            GuardedFieldInfoEx guardedField = new GuardedFieldInfoEx(node, field, attribute, _semanticModel);

                            _guardedFields.Add(guardedField);
                        }
                        else if (string.Equals(attribute.AttributeClass.Name, LockAttributeType.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            LockInfoEx lockField = new LockInfoEx(node,field, attribute, _semanticModel);

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

            public List<LockInfoEx> Locks { get { return _locks; } }
            public List<GuardedFieldInfoEx> GuardedFields { get { return _guardedFields; } }
        }

        public ClassInspectorEx(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _walker = new ClassWalker(_semanticModel);
        }

        public ClassInfoEx GetClassInfo(ClassDeclarationSyntax classDeclaration)
        {
            _walker.Visit(classDeclaration);
            
            ClassInfoEx classInfo = new ClassInfoEx(classDeclaration, _semanticModel);

            classInfo.Locks.AddRange(_walker.Locks);
            classInfo.GuardedFields.AddRange(_walker.GuardedFields);

            foreach (LockInfoEx lockInfo in classInfo.Locks)
            {
                lockInfo.Parent = classInfo;
            }

            foreach (GuardedFieldInfoEx fieldInfo in classInfo.GuardedFields)
            {
                fieldInfo.Parent = classInfo;
            }

            return classInfo;
        }
    }

    public class BaseInfoEx<T> where T : MemberDeclarationSyntax
    {
        private T _declaration;
        private SemanticModel _semanticModel;

        public BaseInfoEx(T declaration, SemanticModel semanticModel)
        {
            _declaration = declaration;
            _semanticModel = semanticModel;
        }

        public T Declaration { get { return _declaration; } }

        public SemanticModel SemanticModel { get { return _semanticModel; } }
    }

    public class ClassInfoEx : BaseInfoEx<ClassDeclarationSyntax>
    {
        private List<GuardedFieldInfoEx> _guardedFields;
        private List<LockInfoEx> _locks;

        public ClassInfoEx(ClassDeclarationSyntax declaration, SemanticModel semanticModel) : base(declaration, semanticModel)
        {
            _guardedFields = new List<GuardedFieldInfoEx>();
            _locks = new List<LockInfoEx>();
        }

        public List<GuardedFieldInfoEx> GuardedFields { get { return _guardedFields; } }
        public List<LockInfoEx> Locks { get { return _locks; } }
    }

    public class AttributeAssociatedFieldInfoEx : BaseInfoEx<FieldDeclarationSyntax>
    {
        private FieldSymbol _symbol;
        private AttributeData _attribute;
        private ClassInfoEx _parent;

        public AttributeAssociatedFieldInfoEx(
            FieldDeclarationSyntax declaration, 
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel) : base(declaration, semanticModel)
        {
            _symbol = symbol;
            _attribute = associatedAttribute;
        }

        public ClassInfoEx Parent { get { return _parent; } set { _parent = value; } }
    }

    public class GuardedFieldInfoEx : AttributeAssociatedFieldInfoEx
    {
        private ClassInfoEx _parent;

        public GuardedFieldInfoEx(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel)
            : base(declaration, symbol, associatedAttribute, semanticModel) {}

    }

    public class LockInfoEx : AttributeAssociatedFieldInfoEx
    {
        private ClassInfoEx _parent;

        public LockInfoEx(
            FieldDeclarationSyntax declaration,
            FieldSymbol symbol,
            AttributeData associatedAttribute,
            SemanticModel semanticModel) 
            : base(declaration, symbol, associatedAttribute, semanticModel) {}
    }
}
