using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldInNonThreadSafeClass : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute == false &&
                classInfo.GuardedFields.Count > 0)
            {
                return new Issue(
                    ErrorCode.GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS,
                    classInfo.Declaration,
                    classInfo.Symbol);
            }

            return null;
        }
    }
}
