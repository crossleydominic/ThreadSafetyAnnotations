using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedMemberRules
{
    internal class GuardedMemberMustBePrivateRule : AnalysisRule<GuardedMemberInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Guarded member must be declared private"; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE; }
        }

        protected override AnalysisResult OnAnalyze(GuardedMemberInfo target)
        {
            return target.IsPrivate ? AnalysisResult.Succeeded : AnalysisResult.Failed;
        }
    }
}

