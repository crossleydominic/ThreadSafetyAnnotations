using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    public class GuardedFieldMustUseLockOnlyOnce : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (GuardedFieldInfo guardedField in classInfo.GuardedFields)
            {
                if (guardedField.DeclaredLockHierarchy.Distinct().Count() != guardedField.DeclaredLockHierarchy.Count)
                {
                    return new Issue(
                        ErrorCode.GUARDED_FIELD_USES_SAME_LOCK_MORE_THAN_ONCE, 
                        guardedField.Declaration,
                        guardedField.Symbol);
                }
            }

            return null;
        }
    }
}
