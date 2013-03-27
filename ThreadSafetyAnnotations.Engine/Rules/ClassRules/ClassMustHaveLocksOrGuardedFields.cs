using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    public class ClassMustHaveLocksOrGuardedFields : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute)
            {
                if (classInfo.GuardedFields.Count == 0 ||
                    classInfo.Locks.Count == 0)
                {
                    return new AnalysisResult(new Issue(
                        ErrorCode.CLASS_MUST_HAVE_LOCKS_OR_GUARDED_FIELDS,
                        classInfo.Declaration,
                        classInfo.Symbol));
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}
