using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedMemberRules
{
    internal class GuardedMemberMustBeProtectedRule : AnalysisRule<GuardedMemberInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Guarded member must be protected by at least one lock"; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK; }
        }

        protected override AnalysisResult OnAnalyze(GuardedMemberInfo target)
        {
            return target.ProtectingLockNames.Count > 0 ? AnalysisResult.Succeeded : AnalysisResult.Failed;
        }
    }
}
