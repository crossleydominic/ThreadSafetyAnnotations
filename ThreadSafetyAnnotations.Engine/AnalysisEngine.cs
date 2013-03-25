using System.Reflection;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;
using ThreadSafetyAnnotations.Engine.Rules;

namespace ThreadSafetyAnnotations.Engine
{
    public class AnalysisEngine
    {
        private CommonSyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        private IAnalysisRuleProvider _ruleProvider;

        public AnalysisEngine(CommonSyntaxTree syntaxTree, SemanticModel semanticModel) : this (syntaxTree, semanticModel, new AnalysisRuleProvider()){}

        public AnalysisEngine(CommonSyntaxTree syntaxTree, SemanticModel semanticModel, IAnalysisRuleProvider ruleProvider)
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

        public bool CanAnalyze
        {
            get
            {
                var diagnostics = _semanticModel.GetDiagnostics();
                var declarationDiagnostics = _semanticModel.GetDeclarationDiagnostics();

                if (diagnostics.Any(d => d.Info.Severity == DiagnosticSeverity.Error) ||
                    declarationDiagnostics.Any(d => d.Info.Severity == DiagnosticSeverity.Error))
                {
                    return false;
                }

                return true;
            }
        }

        public List<Issue> Analyze()
        {
            if(!CanAnalyze)
            {
                throw new InvalidOperationException("Pre-existing errors in compilation");
            }

            List<ClassDeclarationSyntax> classDeclarations = _syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            List<ClassInfo> classInfos = InspectClassDeclarations(classDeclarations);

            List<Issue> issues = new List<Issue>();

            foreach (ClassInfo classInfo in classInfos)
            {
                foreach (IAnalysisRule rule in _ruleProvider.Rules)
                {
                    AnalysisResult result = rule.Analyze(_syntaxTree, _semanticModel, classInfo);

                    if (!result.Success)
                    {
                        issues.AddRange(result.Issues);
                    }
                }
            }

            return issues;
        }

        private List<ClassInfo> InspectClassDeclarations(List<ClassDeclarationSyntax> classDeclarations)
        {
            List<ClassInfo> classInfos = new List<ClassInfo>();

            ClassInspector inspector = new ClassInspector(_semanticModel);

            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                classInfos.Add(inspector.GetClassInfo(classDeclaration));
            }

            return classInfos;
        }
    }
}
