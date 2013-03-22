using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;

namespace ThreadSafetyAnnotations.Engine.Rules.GuardedFieldRules
{
    internal class GuardedFieldMustBeAssociatedWithLock : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            foreach (GuardedFieldInfo guardedField in classInfo.GuardedFields)
            {
                if (guardedField.Attribute.ConstructorArguments.Any() == false)
                {
                    return new Issue(
                        ErrorCode.GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK,
                        guardedField.Declaration,
                        guardedField.Symbol);
                }
            }

            return null;
        }
    }
}
