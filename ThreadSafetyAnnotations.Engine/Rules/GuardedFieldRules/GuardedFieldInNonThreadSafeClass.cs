using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldInNonThreadSafeClass : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute == false &&
                classInfo.GuardedFields.Count > 0)
            {
                return new AnalysisResult(new Issue(
                    ErrorCode.GUARDED_FIELD_IN_A_NON_THREAD_SAFE_CLASS,
                    classInfo.Declaration,
                    classInfo.Symbol));
            }

            return AnalysisResult.Succeeded;
        }
    }
}
