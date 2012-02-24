using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class ClassCannotBePartialRule : AnalysisRule<ClassInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Classes marked with the ThreadSafe attribute cannot be partial."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.CLASS_CANNOT_BE_PARTIAL; }
        }

        protected override AnalysisResult OnAnalyze(ClassInfo target)
        {
            return target.IsPartialClass ? AnalysisResult.Failed : AnalysisResult.Succeeded;
        }
    }
}
