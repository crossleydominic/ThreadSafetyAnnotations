using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    internal abstract class AnalysisRule<T> : IAnalysisRule
        where T : IBaseInfo
    {
        private Type _targetType;

        protected AnalysisRule()
        {
            _targetType = typeof(T);
        }

        protected abstract string RuleViolationMessage { get; }
        protected abstract ErrorCode RuleViolationCode { get; }

        public List<Issue> Analyze(IBaseInfo target)
        {
            List<Issue> issues = new List<Issue>();

            AnalysisResult result = OnAnalyze((T)target);

            if (result == AnalysisResult.Failed)
            {
                issues.Add(new Issue(
                    RuleViolationMessage,
                    RuleViolationCode,
                    target.Declaration,
                    target.Symbol));
            }
            else if (result == AnalysisResult.Inconclusive)
            {
                throw new NotImplementedException();
            }

            return issues;
        }

        protected abstract AnalysisResult OnAnalyze(T target);

        public Type TargetType
        {
            get { return _targetType; }
        }


        public Type TargetTypeEx
        {
            get { throw new NotImplementedException(); }
        }

        public List<Issue> AnalyzeEx(Roslyn.Compilers.CSharp.SyntaxNode node, Roslyn.Compilers.Common.CommonSyntaxTree tree, Roslyn.Compilers.CSharp.SemanticModel model)
        {
            throw new NotImplementedException();
        }
    }
}
