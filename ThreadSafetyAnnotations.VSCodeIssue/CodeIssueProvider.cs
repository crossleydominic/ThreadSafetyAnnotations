using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using ThreadSafetyAnnotations.Engine;

namespace ThreadSafetyAnnotations.VSCodeIssue
{
    [ExportSyntaxNodeCodeIssueProvider("ThreadSafetyAnnotations.VSCodeIssue", LanguageNames.CSharp)]
    class CodeIssueProvider : ICodeIssueProvider
    {
        private readonly ICodeActionEditFactory editFactory;

        [ImportingConstructor]
        public CodeIssueProvider(ICodeActionEditFactory editFactory)
        {
            this.editFactory = editFactory;
        }

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxNode node, CancellationToken cancellationToken)
        {
            //AnalysisEngine engine = new AnalysisEngine(document);
            AnalysisEngine engine = new AnalysisEngine(
                            document.GetSyntaxTree(),
                            document.GetSemanticModel());
            foreach (Issue i in engine.Analzye())
            {
                yield return new CodeIssue(CodeIssue.Severity.Warning, i.SyntaxNode.Span, i.Description);
            }
        }

        #region Unimplemented ICodeIssueProvider members

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxToken token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CodeIssue> GetIssues(IDocument document, CommonSyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
