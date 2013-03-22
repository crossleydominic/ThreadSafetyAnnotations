using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.LockRules
{
    internal class LockInNonThreadSafeClass : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute == false &&
                classInfo.Locks.Count > 0)
            {
                return new Issue(
                    ErrorCode.LOCK_IN_A_NON_THREAD_SAFE_CLASS,
                    classInfo.Declaration,
                    classInfo.Symbol);
            }

            return null;
        }
    }
}
