using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Services;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using Roslyn.Compilers;

namespace ThreadSafetyAnnotations.Engine
{
    public class AnalysisEngine
    {
        //private IDocument _document;
        private CommonSyntaxTree _syntaxTree;
        private ISemanticModel _semanticModel;

        /*public AnalysisEngine(IDocument document)
        {
            #region Input validation

            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            #endregion

            _document = document;
            _syntaxTree = document.GetSyntaxTree();
            _semanticModel = document.GetSemanticModel();
        }*/

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
        }

        public List<Issue> Analzye()
        {
            List<Issue> issues = new List<Issue>();

            List<ClassInfo> classInfos = GetClasses();
            foreach (ClassInfo classInfo in classInfos)
            {
                issues.AddRange(classInfo.Analyze());
            }

            return issues;
            
        }

        private List<ClassInfo> GetClasses()
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
