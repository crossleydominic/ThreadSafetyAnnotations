using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockMustBeSystemObject : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (LockInfo lockInfo in classInfo.Locks)
            {
                if (lockInfo.Symbol.Type.SpecialType != Roslyn.Compilers.SpecialType.System_Object)
                {
                    return new AnalysisResult(new Issue(
                        ErrorCode.LOCK_MUST_BE_SYSTEM_OBJECT,
                        lockInfo.Declaration,
                        lockInfo.Symbol));
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}
