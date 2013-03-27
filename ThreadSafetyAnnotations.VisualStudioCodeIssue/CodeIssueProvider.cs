using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using ThreadSafetyAnnotations.Engine;

namespace ThreadSafetyAnnotations.VisualStudioCodeIssue
{
    [ExportCodeIssueProvider("ThreadSafetyAnnotations.VisualStudioCodeIssue", LanguageNames.CSharp)]
    class CodeIssueProvider : ICodeIssueProvider
    {
        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxNode node, CancellationToken cancellationToken)
        {
            AnalysisEngine engine = new AnalysisEngine(
                   document.GetSyntaxTree(),
                   (SemanticModel)document.GetSemanticModel());

            if (!engine.CanAnalyze)
            {
                yield return null;
            }

            foreach (Issue i in engine.Analyze())
            {
                yield return new CodeIssue(CodeIssueKind.Warning, i.SyntaxNode.Span, i.Description);
            }
        }

        public IEnumerable<Type> SyntaxNodeTypes
        {
            get
            {
                yield return typeof(ClassDeclarationSyntax);
            }
        }

        #region Unimplemented ICodeIssueProvider members

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxToken token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> SyntaxTokenKinds
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
