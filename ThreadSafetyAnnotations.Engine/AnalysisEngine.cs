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
        private IAnalysisRuleProvider _ruleProvider;

        public AnalysisEngine() : this (new AnalysisRuleProvider()){}

        public AnalysisEngine(IAnalysisRuleProvider ruleProvider)
        {
            _ruleProvider = ruleProvider;
        }

        public bool CanAnalyze(CommonSyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var diagnostics = semanticModel.GetDiagnostics();
            var declarationDiagnostics = semanticModel.GetDeclarationDiagnostics();

            if (diagnostics.Any(d => d.Info.Severity == DiagnosticSeverity.Error) ||
                declarationDiagnostics.Any(d => d.Info.Severity == DiagnosticSeverity.Error))
            {
                return false;
            }

            return true;
        }

        public AnalysisResult Analyze(CommonSyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            //TODO: Remove this once Symbol loading has been made defensive for nulls later on
            if(!CanAnalyze(syntaxTree, semanticModel))
            {
                throw new InvalidOperationException("Pre-existing errors in compilation");
            }

            List<ClassDeclarationSyntax> classDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            List<ClassInfo> classInfos = InspectClassDeclarations(semanticModel, classDeclarations);

            List<Issue> issues = new List<Issue>();

            foreach (ClassInfo classInfo in classInfos)
            {
                foreach (IAnalysisRule rule in _ruleProvider.Rules)
                {
                    AnalysisResult result = rule.Analyze(syntaxTree, semanticModel, classInfo);

                    if (!result.Success)
                    {
                        issues.AddRange(result.Issues);
                    }
                }
            }

            if (issues.Count > 0)
            {
                return new AnalysisResult(issues);
            }
            else
            {
                return AnalysisResult.Succeeded;
            }
        }

        private List<ClassInfo> InspectClassDeclarations(SemanticModel semanticModel, List<ClassDeclarationSyntax> classDeclarations)
        {
            List<ClassInfo> classInfos = new List<ClassInfo>();

            ClassInspector inspector = new ClassInspector(semanticModel);

            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                classInfos.Add(inspector.GetClassInfo(classDeclaration));
            }

            return classInfos;
        }
    }
}
