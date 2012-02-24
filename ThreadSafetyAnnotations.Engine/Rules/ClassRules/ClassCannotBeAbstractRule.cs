using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class ClassCannotBeAbstractRule : AnalysisRule<ClassInfo>
    {
        protected override string RuleViolationMessage
        {
            get { return "Classes marked with the ThreadSafe attribute cannot be abstract."; }
        }

        protected override ErrorCode RuleViolationCode
        {
            get { return ErrorCode.CLASS_CANNOT_BE_ABSTRACT; }
        }

        protected override AnalysisResult OnAnalyze(ClassInfo target)
        {
            return target.IsAbstract ? AnalysisResult.Failed : AnalysisResult.Succeeded;
        }
    }
}
