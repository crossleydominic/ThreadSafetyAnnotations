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
    public class GuardedFieldLockHierarchyDeclarationMustNotConflict : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            //TODO: Refactor this whole method later and make it more robust

            if (classInfo.GuardedFields.Count <= 1)
            {
                return AnalysisResult.Succeeded;
            }

            foreach (GuardedFieldInfo first in classInfo.GuardedFields)
            {
                foreach (GuardedFieldInfo second in classInfo.GuardedFields)
                {
                    if (first == second)
                    {
                        continue;
                    }

                    if (LockHierarchy.Conflicts(first.DeclaredLockHierarchy, second.DeclaredLockHierarchy))
                    {
                        return new AnalysisResult(new Issue(
                            ErrorCode.GUARDED_FIELD_LOCK_HIERARCHY_DECLARATION_CONFLICT,
                            second.Declaration,
                            second.Symbol));
                    }
                }
            }

            return AnalysisResult.Succeeded;
        }

    }
}
