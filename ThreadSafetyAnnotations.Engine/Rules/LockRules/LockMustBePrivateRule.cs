using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockMustBePrivateRule : AnalysisRule<LockInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Lock must be declared private"; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.LOCK_IS_NOT_PRIVATE; }
        }

        protected override bool OnAnalyze(LockInfo target)
        {
            return target.IsPrivate;
        }
    }
}
