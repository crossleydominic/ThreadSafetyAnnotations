using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class GuardedMemberInNonThreadSafeClassRule : AnalysisRule<ClassInfo>
    {
        protected override AnalysisResult OnAnalyze(ClassInfo target)
        {
            if (target.GuardedMembers.Count == 0)
            {
                return AnalysisResult.Succeeded;
            }
            else
            {
                return (target.IsMarkedWithThreadSafeAttribute ? AnalysisResult.Succeeded : AnalysisResult.Failed);
            }
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
