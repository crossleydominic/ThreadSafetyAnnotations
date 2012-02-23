using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class LockInNonThreadSafeClassRule : AnalysisRule<ClassInfo>
    {
        protected override bool OnAnalyze(ClassInfo target)
        {
            return target.Locks.Count > 0 &&
                target.IsMarkedWithThreadSafeAttribute == true;
        }

        protected override string RuleViolationMessage
        {
            get { return "Class is not marked with the ThreadSafeAttribute but contains a lock."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.LOCK_IN_A_NON_THREAD_SAFE_CLASS; }
        }
    }
}
