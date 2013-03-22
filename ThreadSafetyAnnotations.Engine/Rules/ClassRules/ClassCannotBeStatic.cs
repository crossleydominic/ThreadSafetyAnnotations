using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Rules.ClassRules
{
    internal class ClassCannotBeStatic : IAnalysisRule
    {
        public Issue AnalyzeEx(CommonSyntaxTree tree, SemanticModel model, ClassInfo classInfo)
        {
            if (classInfo.HasThreadSafeAttribute &&
                classInfo.Symbol.IsStatic)
            {
                return new Issue(
                    ErrorCode.CLASS_CANNOT_BE_STATIC,
                    classInfo.Declaration,
                    classInfo.Symbol);
            }

            return null;
        }
    }
}
