using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockMustBeSystemObjectRule : AnalysisRule<LockInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Lock must be of type System.Object"; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT; }
        }

        protected override AnalysisResult OnAnalyze(LockInfo target)
        {
            return target.IsSystemObject ? AnalysisResult.Succeeded : AnalysisResult.Failed;
        }
    }
}
