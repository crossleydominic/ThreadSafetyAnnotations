using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class GuardedMembersReferenceKnownLocksRule : AnalysisRule<ClassInfo>
    {
        protected override AnalysisResult OnAnalyze(ClassInfo target)
        {            
            //Check that all guards reference a named lock.
            foreach (GuardedMemberInfo guardedMember in target.GuardedMembers)
            {
                foreach (string protectingLockname in guardedMember.ProtectingLockNames)
                {
                    if (!target.Locks.Any(l => l.LockName == protectingLockname))
                    {
                        return AnalysisResult.Failed;
                    }
                }
            }

            return AnalysisResult.Succeeded;
        }

        protected override string RuleViolationMessage
        {
            get { return "Declared member references unknown lock "; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK; }
        }

    }
}
