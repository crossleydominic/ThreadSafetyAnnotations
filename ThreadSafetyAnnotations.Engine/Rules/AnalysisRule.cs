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

            if (!OnAnalyze((T)target))
            {
                issues.Add(new Issue(
                    RuleViolationMessage,
                    RuleViolationCode,
                    target.Declaration,
                    target.Symbol));
            }

            return issues;
        }

        protected abstract bool OnAnalyze(T target);

        public Type TargetType
        {
            get { return _targetType; }
        }
    }
}
