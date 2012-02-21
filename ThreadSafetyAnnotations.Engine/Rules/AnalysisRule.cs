using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules
{
    internal abstract class AnalysisRule<T>
    {
        protected abstract string RuleViolationMessage { get; }
        protected abstract ErrorCode RuleViolationCode { get; }
        
        public List<Issue> Analyze(T target)
        {
            List<Issue> issues = new List<Issue>();
            
            if (!OnAnalyze(target))
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
    }
}
