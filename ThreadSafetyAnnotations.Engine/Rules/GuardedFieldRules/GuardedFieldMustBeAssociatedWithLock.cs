using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldMustBeAssociatedWithLock : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (GuardedFieldInfo guardedField in classInfo.GuardedFields)
            {
                if (guardedField.DeclaredLockHierarchy.Any() == false)
                {
                    return  new AnalysisResult(new Issue(
                        ErrorCode.GUARDED_FIELD_NOT_ASSOCIATED_WITH_A_LOCK,
                        guardedField.Declaration,
                        guardedField.Symbol));
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}
