using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockMustBePrivate : IAnalysisRule
    {
        public AnalysisResult AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (LockInfo lockInfo in classInfo.Locks)
            {
                if (lockInfo.Symbol.DeclaredAccessibility != Accessibility.Private)
                {
                    return new AnalysisResult(new Issue(
                        ErrorCode.LOCK_IS_NOT_PRIVATE,
                        lockInfo.Declaration,
                        lockInfo.Symbol));
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}
