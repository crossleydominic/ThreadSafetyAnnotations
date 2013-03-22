using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldMustBePrivate : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (GuardedFieldInfo fieldInfo in classInfo.GuardedFields)
            {
                if (fieldInfo.Symbol.DeclaredAccessibility != Accessibility.Private)
                {
                    return new Issue(
                        ErrorCode.GUARDED_MEMBER_IS_NOT_PRIVATE,
                        fieldInfo.Declaration,
                        fieldInfo.Symbol);
                }
            }

            return null;
        }
    }
}

