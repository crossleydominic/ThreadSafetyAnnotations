using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class LockProtectsGuardedMember : AnalysisRule<ClassInfo>
    {
        protected override bool OnAnalyze(ClassInfo target)
        {
            //Check that all locks actually protect something
            foreach (LockInfo lockInfo in target.Locks)
            {
                if (!target.GuardedMembers.Any(g => g.ProtectingLockNames.Any(p => p == lockInfo.LockName)))
                {
                    return false;
                }
            }

            return true;
        }

        protected override string RuleViolationMessage
        {
            get { return "Lock is not guarding any member."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.LOCK_PROTECTS_NOTHING; }
        }
    }
}
