using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class ClassCannotBeStaticRule : AnalysisRule<ClassInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Classes marked with the ThreadSafe attribute cannot be static."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.CLASS_CANNOT_BE_STATIC; }
        }

        protected override AnalysisResult OnAnalyze(ClassInfo target)
        {
            return target.IsStatic ? AnalysisResult.Failed : AnalysisResult.Succeeded;
        }
    }
}
