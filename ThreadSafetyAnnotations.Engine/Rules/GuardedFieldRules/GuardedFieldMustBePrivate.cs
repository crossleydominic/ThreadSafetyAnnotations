using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldMustBePrivate : IAnalysisRule
    {
        public AnalysisResult Analyze(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (GuardedFieldInfo fieldInfo in classInfo.GuardedFields)
            {
                if (fieldInfo.Symbol.DeclaredAccessibility != Accessibility.Private)
                {
                    return new AnalysisResult(new Issue(
                        ErrorCode.GUARDED_FIELD_IS_NOT_PRIVATE,
                        fieldInfo.Declaration,
                        fieldInfo.Symbol));
                }
            }

            return AnalysisResult.Succeeded;
        }
    }
}

