using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class GuardedMemberInNonThreadSafeClassRule : ClassRule
    {
        protected override bool OnAnalyze(ClassInfo target)
        {
            return target.GuardedMembers.Count > 0 &&
               target.IsMarkedWithThreadSafeAttribute == false;
        }

        protected override string RuleViolationMessage
        {
            get { return "Class is not marked with the ThreadSafeAttribute but contains a guarded member."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS; }
        }
    }
}
