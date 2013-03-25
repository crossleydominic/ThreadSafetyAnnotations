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

                    List<string> firstList = first.DeclaredLockHierarchy.ToList();
                    List<string> secondList = second.DeclaredLockHierarchy.ToList();

                    List<string> longer, shorter;

                    if (firstList.Count >= secondList.Count)
                    {
                        longer = firstList;
                        shorter = secondList;
                    }
                    else
                    {
                        longer = secondList;
                        shorter = firstList;
                    }

                    for (int i = 0; i < shorter.Count; i++)
                    {
                        string currentShorter = shorter[i];

                        int index = longer.IndexOf(currentShorter);
                        if (index == -1)
                        {
                            continue;
                        }

                        if (index < i)
                        {
                            return new AnalysisResult( new Issue(
                                ErrorCode.GUARDED_FIELD_LOCK_HIERARCHY_DECLARATION_CONFLICT, 
                                second.Declaration,
                                second.Symbol));
                        }
                    }
                }
            }

            return AnalysisResult.Succeeded;
        }

    }
}
