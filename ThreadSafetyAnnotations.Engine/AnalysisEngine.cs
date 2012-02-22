using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Services;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using Roslyn.Compilers;
using ThreadSafetyAnnotations.Engine.Rules.ClassRules;

namespace ThreadSafetyAnnotations.Engine
{
    public class AnalysisEngine
    {
        //private IDocument _document;
        private CommonSyntaxTree _syntaxTree;
        private ISemanticModel _semanticModel;

        private List<ClassRule> _classRules;

        public AnalysisEngine(CommonSyntaxTree syntaxTree, ISemanticModel semanticModel)
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

            _classRules = new List<ClassRule>()
            {
                new GuardedMemberInNonThreadSafeClassRule(),
                new LockInNonThreadSafeClassRule(),
                new GuardedMembersReferenceKnownLocksRule(),
                new LockProtectsGuardedMember()
            };
        }

        public List<Issue> Analzye()
        {
            List<Issue> issues = new List<Issue>();

            List<ClassInfo> classInfos = DiscoverInformation();
            /*foreach (ClassInfo classInfo in classInfos)
            {
                issues.AddRange(classInfo.Analyze());
            }*/

            foreach (ClassInfo classInfo in classInfos)
            {
                foreach (ClassRule rule in _classRules)
                {
                    issues.AddRange(rule.Analyze(classInfo));
                }
            }

            return issues;
            
        }

        private List<ClassInfo> DiscoverInformation()
        {
            List<ClassInfo> classes = new List<ClassInfo>();

            foreach (ClassDeclarationSyntax classDeclaration in _syntaxTree.Root.DescendentNodes().OfType<ClassDeclarationSyntax>())
            {
                classes.Add(new ClassInfo(classDeclaration, _semanticModel));
            }

            return classes;
        }
    }
}
